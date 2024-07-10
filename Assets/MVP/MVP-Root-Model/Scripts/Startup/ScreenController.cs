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

        public void Start()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            string currentSceneName = activeScene.name;

            ScreenModelMap screenModelMap = SceneNameToEnum(currentSceneName);

            RunModel(screenModelMap).Forget();
        }

        public async UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null)
        {
            Debug.Log("Run Model: " + screenModelMap);

            await _semaphoreSlim.WaitAsync();

            try
            {
                string newSceneName = screenModelMap.ToString();
                Debug.Log("Open Scene: " + newSceneName);
                await _sceneService.OnLoadSceneAsync(newSceneName);

                ISceneInstaller sceneInstaller = FindSceneInstaller();
                var sceneLifetimeScope = sceneInstaller.CreateSceneLifetimeScope(LifetimeScope.Find<RootLifetimeScope>());

                _currentModel = _screenTypeMapper.Resolve(screenModelMap, sceneLifetimeScope);

                await _currentModel.Run(param);
                await _currentModel.Stop();
                _currentModel.Dispose();
                sceneLifetimeScope.Dispose();
            }
            finally { _semaphoreSlim.Release(); }
        }

        private static ISceneInstaller FindSceneInstaller()
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

        private static ScreenModelMap SceneNameToEnum(string sceneName)
        {
            if (System.Enum.TryParse(sceneName, out ScreenModelMap result)) 
                return result;            
            else
            {
                Debug.LogError($"Failed to convert scene name {sceneName} to ScreenModelMap");
                return ScreenModelMap.StartGame; 
            }
        }
    }
}
