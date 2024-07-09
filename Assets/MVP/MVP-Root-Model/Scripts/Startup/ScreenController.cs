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
                //if (!string.IsNullOrEmpty(_currentSceneName)) //TODO NE NADO
                //{
                //    var currentScene = SceneManager.GetSceneByName(_currentSceneName);
                //    if (currentScene.IsValid() && currentScene.isLoaded)
                //    {
                //        var unloadOperation = SceneManager.UnloadSceneAsync(_currentSceneName);

                //        if (unloadOperation != null)    
                //            await unloadOperation;                        
                //    }
                //}

                await _sceneService.OnLoadSceneAsync(newSceneName);

                _currentSceneName = newSceneName;
                ISceneInstaller sceneInstaller = FindSceneInstaller();
                var sceneLifetimeScope = sceneInstaller.CreateSceneLifetimeScope(LifetimeScope.Find<RootLifetimeScope>());

                _currentModel = _screenTypeMapper.Resolve(screenModelMap, sceneLifetimeScope);

                await _currentModel.Run(param);
                await _currentModel.Stop();
                _currentModel.Dispose();
                sceneLifetimeScope.Dispose();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
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
