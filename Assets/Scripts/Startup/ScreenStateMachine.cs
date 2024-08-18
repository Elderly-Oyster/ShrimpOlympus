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

        // SemaphoreSlim to ensure only one thread can execute the RunPresenter method at a time
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private IScreenPresenter _currentPresenter; //TODO Вот эта сущность главная

        public void Start()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            string currentSceneName = activeScene.name;
            ScreenPresenterMap? screenModelMap = SceneNameToEnum(currentSceneName);
            
            if (screenModelMap != null)
                RunPresenter((ScreenPresenterMap)screenModelMap).Forget();
            else
            {
                _sceneInstallerService.
                    CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), false);
            }
        }

        public async UniTaskVoid RunPresenter(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            Debug.Log("Run Presenter: " + screenPresenterMap);
            // Wait until the semaphore is available (only one thread can pass)
            await _semaphoreSlim.WaitAsync();

            try
            {
                await _sceneService.LoadScenesForModule(screenPresenterMap);

                var sceneLifetimeScope = _sceneInstallerService.
                    CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), true);

                _currentPresenter = _screenTypeMapper.Resolve(screenPresenterMap, sceneLifetimeScope.Container);

                ModuleChanged?.Invoke(sceneLifetimeScope.Container);

                await _currentPresenter.Enter(param);
                await _currentPresenter.Execute();
                await _currentPresenter.Exit();
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
