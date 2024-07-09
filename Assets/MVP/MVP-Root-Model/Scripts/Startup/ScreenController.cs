using System.Threading;
using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public class ScreenController : IRootController, IStartable
    {
        [Inject] private ScreenTypeMapper _screenTypeMapper;
        [Inject] private SceneService _sceneService;
        
        private readonly SemaphoreSlim _semaphoreSlim = new (1, 1);
        private IScreenModel _currentModel;
        private string _currentSceneName;

        public void Start()
        {
            Debug.Log("Try Run StartGame Mode");
            RunModel(ScreenModelMap.StartGame).Forget();
        }

        public async UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null)
        {
            await _semaphoreSlim.WaitAsync();
            
            try
            {
                string newSceneName = screenModelMap.ToString();
                if (!string.IsNullOrEmpty(_currentSceneName))
                {
                    //Тут какая-то херня, сцена тупо не хочет выгружаться
                    Debug.Log($"Unloading current scene: {_currentSceneName}");
                    var currentScene = SceneManager.GetSceneByName(_currentSceneName);
                    if (currentScene.isLoaded)
                    {
                        Debug.Log($"Current scene is loaded: {_currentSceneName}");
                        var unloadOperation = SceneManager.UnloadSceneAsync(_currentSceneName);
                        if (unloadOperation != null)
                            await unloadOperation;
                        else
                            Debug.LogError($"Failed to get unload operation for scene: {_currentSceneName}");
                    }
                    else
                        Debug.LogWarning($"Current scene is not loaded: {_currentSceneName}");

                    await _sceneService.OnLoadSceneAsync(newSceneName);
                }
                
                _currentSceneName = newSceneName;
                ISceneInstaller sceneInstaller = FindSceneInstaller();
                var sceneLifetimeScope = sceneInstaller.
                    CreateSceneLifetimeScope(LifetimeScope.Find<RootLifetimeScope>());
                
                Debug.Log("Try Resolve StartGame Mode");
                _currentModel = _screenTypeMapper.Resolve(screenModelMap, sceneLifetimeScope);
                Debug.Log("Resolved StartGame Mode: " + _currentModel);

                await _currentModel.Run(param);
                await _currentModel.Stop();
                _currentModel.Dispose();
                sceneLifetimeScope.Dispose();
            }
            finally { _semaphoreSlim.Release(); }
        }

        public ISceneInstaller FindSceneInstaller()
        {
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        
            foreach (GameObject rootObject in rootObjects)
            {
                ISceneInstaller sceneInstaller = rootObject.GetComponent<ISceneInstaller>();
                if (sceneInstaller != null)
                    return sceneInstaller;
            }
            
            return null;
        }
    }
}
