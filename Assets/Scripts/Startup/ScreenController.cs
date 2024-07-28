using System;
using System.Threading;
using Core;
using Core.Popup.Scripts;
using Core.Popup.Scripts.PopupTest;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Startup
{
    public class ScreenController : IScreenController, IStartable
    {
        [Inject] private readonly SceneInstallerService _sceneInstallerService;
        [Inject] private readonly ScreenTypeMapper _screenTypeMapper;
        [Inject] private readonly SceneService _sceneService;
        [Inject] private readonly IObjectResolver _resolver;
        
        public event Action<IObjectResolver> ModuleChanged;


        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private IScreenModel _currentModel;

        public void Start()
        {
            /*_sceneService.AddStaticAdditiveScene(AdditiveScenesMap.PopupsManager);
            _sceneService.LoadStaticScenes().Forget();*/
            
            Scene activeScene = SceneManager.GetActiveScene();
            string currentSceneName = activeScene.name;
            ScreenModelMap? screenModelMap = SceneNameToEnum(currentSceneName);
            if (screenModelMap != null)
                RunModel((ScreenModelMap)screenModelMap).Forget();
            else
            {
                _sceneInstallerService.
                    CombineScenes(LifetimeScope.Find<RootLifetimeScope>());
            }
        }

        public async UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null)
        {
            Debug.Log("Run Model: " + screenModelMap);
            await _semaphoreSlim.WaitAsync();

            try
            {
                await _sceneService.LoadScenesForModule(screenModelMap);

                var sceneLifetimeScope = _sceneInstallerService.
                    CombineScenes(LifetimeScope.Find<RootLifetimeScope>());

                _currentModel = _screenTypeMapper.Resolve(screenModelMap, sceneLifetimeScope.Container);

                ModuleChanged?.Invoke(sceneLifetimeScope.Container);

                await _currentModel.Run(param);
                await _currentModel.Stop();
                _currentModel.Dispose();
                sceneLifetimeScope.Dispose();
            }
            finally { _semaphoreSlim.Release(); }
        }
        
        private static ScreenModelMap? SceneNameToEnum(string sceneName)
        {
            if (Enum.TryParse(sceneName, out ScreenModelMap result))
                return result;
            return null;
        }
    }
}
