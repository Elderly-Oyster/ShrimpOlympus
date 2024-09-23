using System;
using System.Linq;
using System.Threading;
using Core;
using Core.MVP;
using Cysharp.Threading.Tasks;
using Modules.Additional.LoadingSplashScreen.Scripts;
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

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        public event Action<IObjectResolver> ModuleChanged;
        public IScreenPresenter CurrentPresenter { get; private set; }
        
        public void Start() => RunScreen(SceneManager.GetActiveScene().name);

        private void RunScreen(string sceneName, object param = null)
        {
            ScreenPresenterMap? screenModelMap = SceneNameToEnum(sceneName);
            
            if (screenModelMap != null)
                RunScreen((ScreenPresenterMap)screenModelMap, param).Forget();
            else
            {
                _sceneService.AddActiveScene(sceneName);
                _sceneInstallerService.
                    CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), false);
            }
        }

        public async UniTaskVoid RunScreen(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            Debug.Log("Run Screen: " + screenPresenterMap);
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (_sceneService.GetAdditionalScenes(screenPresenterMap).Any() /*|| CurrentPresenter.HaveService*/)
                    await RunPresenterWithSplashScreen(screenPresenterMap, param);
                else
                {
                    await _sceneService.LoadScenesForModule(screenPresenterMap);
                    await _sceneService.UnloadUnusedScenesAsync();

                    var sceneLifetimeScope =
                        _sceneInstallerService.CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), true);

                    CurrentPresenter = _screenTypeMapper.Resolve(screenPresenterMap, sceneLifetimeScope.Container);

                    //await ShowSplashScreen(); //TODO

                    ModuleChanged?.Invoke(sceneLifetimeScope.Container);

                    await CurrentPresenter.Enter(param);
                    await CurrentPresenter.Execute();
                    await CurrentPresenter.Exit();
                    CurrentPresenter.Dispose();
                    sceneLifetimeScope.Dispose();
                }
            }
            finally { _semaphoreSlim.Release(); }
        }
        
        private async UniTask RunPresenterWithSplashScreen(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            await _splashScreenService.ShowSplashScreen();
            
            _sceneService.LoadScenesForModule(screenPresenterMap).Forget();

            _sceneService.ScenesLoadProgress += _splashScreenService.UpdateScenesProgress;

            await _splashScreenService.ExecuteSplashScreen();
            await _sceneService.UnloadUnusedScenesAsync();

            var sceneLifetimeScope =
                _sceneInstallerService.CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), true);

            CurrentPresenter = _screenTypeMapper.Resolve(screenPresenterMap, sceneLifetimeScope.Container);
            
            ModuleChanged?.Invoke(sceneLifetimeScope.Container);

            await CurrentPresenter.Enter(param);
            await CurrentPresenter.Execute();
            await CurrentPresenter.Exit();
            CurrentPresenter.Dispose();
            sceneLifetimeScope.Dispose();
        }

        private static ScreenPresenterMap? SceneNameToEnum(string sceneName)
        {
            if (Enum.TryParse(sceneName, out ScreenPresenterMap result)) return result;
            return null;
        }
    }
}
