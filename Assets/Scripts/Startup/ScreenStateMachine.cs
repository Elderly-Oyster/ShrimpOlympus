using System;
using System.Threading;
using Core;
using Core.MVVM;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Startup
{
    public class ScreenStateMachine : IScreenStateMachine, IStartable
    {
        [Inject] private readonly SceneInstallerService _sceneInstallerService;
        [Inject] private readonly ScreenTypeMapper _screenTypeMapper;
        [Inject] private readonly SceneService _sceneService;
        [Inject] private readonly IObjectResolver _resolver;
        
        public event Action<IObjectResolver> ModuleChanged;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private IScreenModel _currentPresenter; //TODO Вот эта сущность главная

        public void Start()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            string currentSceneName = activeScene.name;
            ScreenPresenterMap? screenModelMap = SceneNameToEnum(currentSceneName);
            
            if (screenModelMap != null)
                RunViewModel((ScreenPresenterMap)screenModelMap).Forget();
            else
            {
                _sceneInstallerService.
                    CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), false);
            }
        }

        public async UniTaskVoid RunViewModel(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            Debug.Log("Run Model: " + screenPresenterMap);
            await _semaphoreSlim.WaitAsync();

            try
            {
                await _sceneService.LoadScenesForModule(screenPresenterMap);

                var sceneLifetimeScope = _sceneInstallerService.
                    CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), true);

                _currentPresenter = _screenTypeMapper.Resolve(screenPresenterMap, sceneLifetimeScope.Container);

                ModuleChanged?.Invoke(sceneLifetimeScope.Container);

                await _currentPresenter.Run(param);
                await _currentPresenter.Stop();
                _currentPresenter.Dispose();
                sceneLifetimeScope.Dispose();
            }
            finally { _semaphoreSlim.Release(); }
        }
        
        private static ScreenPresenterMap? SceneNameToEnum(string sceneName)
        {
            if (Enum.TryParse(sceneName, out ScreenPresenterMap result))
                return result;
            return null;
        }
    }
}
