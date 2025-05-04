using System;
using System.Threading;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Modules.MVP;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Services;
using CodeBase.Services.SceneInstallerService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Implementation.Infrastructure
{
    public class ModuleStateMachine : IScreenStateMachine, IStartable
    {
        [Inject] private readonly AudioListenerService _audioListenerService;
        [Inject] private readonly SceneInstallerService _sceneInstallerService;
        [Inject] private readonly ModuleTypeMapper _moduleTypeMapper;
        [Inject] private readonly SceneService _sceneService;
        [Inject] private readonly IObjectResolver _resolver;
        
        // reducing the number of threads to one
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1); 
        
        public ModulesMap CurrentModulesMap { get; private set; } = ModulesMap.None;
        public IScreenPresenter CurrentPresenter { get; private set; }

        private IModuleController CurrentModuleController { get; set; }
        
        public void Start() => RunScreen(SceneManager.GetActiveScene().name);

        private void RunScreen(string sceneName, object param = null)
        {
            ModulesMap? screenModelMap = SceneNameToEnum(sceneName);
            
            if (screenModelMap != null)
                RunScreen((ModulesMap)screenModelMap, param).Forget(); 
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
        /// <param name="modulesMap">Type of the screen.</param>
        /// <param name="param">Parameters to pass to Presenter.</param>
        public async UniTaskVoid RunScreen(ModulesMap modulesMap, object param = null)
        {
            if (CheckIsSameScreen(modulesMap))
            {
                Debug.LogWarning("⚠️ The same screen is already active.");
                return;
            }
            
            await _semaphoreSlim.WaitAsync(); //Asynchronously waits to enter the SemaphoreSlim.
            try
            {
                Debug.Log("currentScreenPresenter - " + modulesMap);
                CurrentModulesMap = modulesMap;

                await _sceneService.LoadScenesForModule(CurrentModulesMap);
                await _sceneService.UnloadUnusedScenesAsync();

                // creates children for the root installer
                var sceneLifetimeScope =
                    _sceneInstallerService.CombineScenes(LifetimeScope.Find<RootLifetimeScope>(), true);

                CurrentModuleController =
                    _moduleTypeMapper.ResolveModuleController(CurrentModulesMap, sceneLifetimeScope.Container);

                _audioListenerService.EnsureAudioListenerExists(sceneLifetimeScope.Container);

                await CurrentModuleController.Enter(param);
                await CurrentModuleController.Execute();
                await CurrentModuleController.Exit();

                CurrentModuleController.Dispose();

                sceneLifetimeScope.Dispose(); // only children lifeTimeScopes are destroyed
            }
            // catch (Exception e)
            // {
            //     Debug.LogError("Nothing works, good luck figuring out what's wrong. Here is your beloved exception: " + e); 
            // }
            finally
            {
                _semaphoreSlim.Release(); 
            }
        }
        
        /// <summary>
        /// Checks if the requested screen is already active.
        /// </summary>
        private bool CheckIsSameScreen(ModulesMap screenViewModelMap) => 
            screenViewModelMap == CurrentModulesMap;

        //tries to convert screen name in string to its name in enum. Can return null if the sceneName is not found
        private static ModulesMap? SceneNameToEnum(string sceneName)
        {
            if (Enum.TryParse(sceneName, out ModulesMap result)) return result;
            return null;
        }
    }
}
