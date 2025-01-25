using System;
using System.Threading;
using CodeBase.Core.Modules;
using CodeBase.Core.Root;
using CodeBase.Services;
using CodeBase.Services.SceneInstallerService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Implementation.Root.ScreenStateMachine
{
    public class ScreenStateMachine : IScreenStateMachine, IStartable
    {
        [Inject] private readonly AudioListenerService _audioListenerService;
        [Inject] private readonly SceneInstallerService _sceneInstallerService;
        [Inject] private readonly ScreenTypeMapper _screenTypeMapper;
        [Inject] private readonly SceneService _sceneService;
        [Inject] private readonly IObjectResolver _resolver;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1); // reducing the number of threads to one
        public IScreenPresenter CurrentPresenter { get; private set; }
        
        
        public void Start() => RunScreen(SceneManager.GetActiveScene().name);

        private void RunScreen(string sceneName, object param = null)
        {
            ScreenPresenterMap? screenModelMap = SceneNameToEnum(sceneName);
            
            if (screenModelMap != null)
                RunScreen((ScreenPresenterMap)screenModelMap, param).Forget();  // overload method
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
            await _semaphoreSlim.WaitAsync(); //Asynchronously waits to enter the SemaphoreSlim.

            try
            {
                await _sceneService.LoadScenesForModule(screenPresenterMap);
                await _sceneService.UnloadUnusedScenesAsync();

                // creates children for the root installer
                var sceneLifetimeScope = _sceneInstallerService.
                    CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), true);  
                
                // responsible for resolving (or instantiating) the appropriate screen presenter for the
                // specified screenPresenterMap, using a dependency injection container provided by sceneLifetimeScope
                CurrentPresenter = _screenTypeMapper.Resolve(screenPresenterMap, sceneLifetimeScope.Container);
                
                _audioListenerService.EnsureAudioListenerExists(sceneLifetimeScope.Container);
                
                await CurrentPresenter.Enter(param);
                await CurrentPresenter.Execute();
                await CurrentPresenter.Exit();
                CurrentPresenter.Dispose();
                sceneLifetimeScope.Dispose(); // only children lifeTimeScopes are destroyed
            }
            finally { _semaphoreSlim.Release(); }
        }

        //tries to convert screen name in string to its name in enum. Can return null if the sceneName is not found
        private static ScreenPresenterMap? SceneNameToEnum(string sceneName)
        {
            if (Enum.TryParse(sceneName, out ScreenPresenterMap result)) return result;
            return null;
        }
    }
}
