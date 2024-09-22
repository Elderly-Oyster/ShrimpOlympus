using System;
using System.Linq;
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
        [Inject] private readonly SplashScreenService _splashScreenService;
        [Inject] private readonly IObjectResolver _resolver;

        // SemaphoreSlim to ensure only one thread can execute the RunPresenter method at a time
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        public event Action<IObjectResolver> ModuleChanged;
        public IScreenPresenter CurrentPresenter { get; private set; } 

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
                    CombineActiveScenes(LifetimeScope.Find<RootLifetimeScope>(), false);
            }
        }

        public async UniTaskVoid RunPresenter(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            Debug.Log("Run Presenter: " + screenPresenterMap);
            // Wait until the semaphore is available (only one thread can pass)
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (_sceneService.GetAdditionalScenes(screenPresenterMap).Count() != 0 /*|| CurrentPresenter.HaveService*/)
                    await RunPresenterWithSplashScreen(screenPresenterMap, param);
                else
                    await RunPresenterWithoutSplashScreen(screenPresenterMap, param);
            }
            finally { _semaphoreSlim.Release(); }
        }

        private async UniTask RunPresenterWithoutSplashScreen(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            await _sceneService.LoadScenesForModule(screenPresenterMap);
            // await _sceneService.UnloadUnusedScenesAsync(); //TODO Разобратсья

            var sceneLifetimeScope = _sceneInstallerService.
                CombineActiveScenes(LifetimeScope.Find<RootLifetimeScope>(), true);
                
            CurrentPresenter = _screenTypeMapper.Resolve(screenPresenterMap, sceneLifetimeScope.Container);
            
            ModuleChanged?.Invoke(sceneLifetimeScope.Container);
                
            await CurrentPresenter.Enter(param);
            await CurrentPresenter.Execute();
            await CurrentPresenter.Exit();
            CurrentPresenter.Dispose();
            sceneLifetimeScope.Dispose();
        }

        private async UniTask RunPresenterWithSplashScreen(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            await _sceneService.LoadScenesForModule(screenPresenterMap);
            // await _sceneService.UnloadUnusedScenesAsync(); //TODO Разобратсья

            var sceneLifetimeScope = _sceneInstallerService.
                CombineActiveScenes(LifetimeScope.Find<RootLifetimeScope>(), true);
                
            CurrentPresenter = _screenTypeMapper.Resolve(screenPresenterMap, sceneLifetimeScope.Container);

            //await ShowSplashScreen(); //TODO
                
            ModuleChanged?.Invoke(sceneLifetimeScope.Container);
                
            await CurrentPresenter.Enter(param);
            await CurrentPresenter.Execute();
            await CurrentPresenter.Exit();
            CurrentPresenter.Dispose();
            sceneLifetimeScope.Dispose();
        }
        
        private static ScreenPresenterMap? SceneNameToEnum(string sceneName)
        {
            if (Enum.TryParse(sceneName, out ScreenPresenterMap result))
                return result;
            return null;
        }
    }

    internal class SplashScreenService
    {
        public void ShowSplashScreen()
        {
            
        }
    }
}
