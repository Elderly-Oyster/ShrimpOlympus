using System;
using System.Threading;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Services;
using CodeBase.Services.SceneInstallerService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Implementation.Infrastructure
{
    public class ScreenStateMachine : IScreenStateMachine, IStartable
    {
        [Inject] private readonly AudioListenerService _audioListenerService;
        [Inject] private readonly SceneInstallerService _sceneInstallerService;
        [Inject] private readonly ScreenTypeMapper _screenTypeMapper;
        [Inject] private readonly SceneService _sceneService;
        [Inject] private readonly IObjectResolver _resolver;
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1); // reducing the number of threads to one
        
        public ScreenPresenterMap CurrentScreenPresenterMap { get; private set; } = ScreenPresenterMap.None;
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

        /// <summary>
        /// Launches a new screen state (only after the previous state finishes execution).
        /// </summary>
        /// <param name="screenPresenterMap">Type of the screen.</param>
        /// <param name="param">Parameters to pass to Presenter.</param>
        public async UniTaskVoid RunScreen(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            if (CheckIsSameScreen(screenPresenterMap))
            {
                Debug.LogWarning("⚠️ The same screen is already active.");
                return;
            }
            
            await _semaphoreSlim.WaitAsync(); //Asynchronously waits to enter the SemaphoreSlim.
            try
            {
                Debug.Log("currentScreenPresetner - " + screenPresenterMap);
                CurrentScreenPresenterMap = screenPresenterMap;

                await _sceneService.LoadScenesForModule(CurrentScreenPresenterMap);
                await _sceneService.UnloadUnusedScenesAsync();

                // creates children for the root installer
                var sceneLifetimeScope =
                    _sceneInstallerService.CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), true);
                
                CurrentPresenter = _screenTypeMapper.Resolve(CurrentScreenPresenterMap, sceneLifetimeScope.Container);

                _audioListenerService.EnsureAudioListenerExists(sceneLifetimeScope.Container);

                await CurrentPresenter.Enter(param);
                await CurrentPresenter.Execute();
                await CurrentPresenter.Exit();

                CurrentPresenter.Dispose();
                sceneLifetimeScope.Dispose(); // only children lifeTimeScopes are destroyed
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
        
        /// <summary>
        /// Checks if the requested screen is already active.
        /// </summary>
        private bool CheckIsSameScreen(ScreenPresenterMap screenViewModelMap)
        {
            return screenViewModelMap == CurrentScreenPresenterMap;
        }

        //tries to convert screen name in string to its name in enum. Can return null if the sceneName is not found
        private static ScreenPresenterMap? SceneNameToEnum(string sceneName)
        {
            if (Enum.TryParse(sceneName, out ScreenPresenterMap result)) return result;
            return null;
        }
    }
}
