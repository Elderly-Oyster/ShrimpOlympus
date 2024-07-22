using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Services;
using System;
using System.Threading;
using MVP.MVP_Root_Model.Scripts.Core.EventMediatorSystem;
using MVP.MVP_Root_Model.Scripts.Core.Views;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public class ScreenController : IScreenController, IStartable
    {
        //[Inject] private readonly PopupHub _popupHub;
        //[Inject] private RootCanvas _rootCanvas;

        [Inject] private readonly SceneInstallerManager _sceneInstallerManager;
        [Inject] private readonly ScreenTypeMapper _screenTypeMapper;
        [Inject] private readonly SceneService _sceneService;
        [Inject] private readonly IObjectResolver _resolver;

        public event Action<IObjectResolver> ModuleChanged;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private IScreenModel _currentModel;

        public void Start()
        {
            /*var x = _resolver.TryResolve(out BasePopupFactory<FirstPopup> y);
            Debug.Log(y);*/ 
            //Debug.Log(_rootCanvas);

            Scene activeScene = SceneManager.GetActiveScene();
            string currentSceneName = activeScene.name;
            ScreenModelMap? screenModelMap = SceneNameToEnum(currentSceneName);
            if (screenModelMap != null)
                RunModel((ScreenModelMap)screenModelMap).Forget();
        }

        public async UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null)
        {
            Debug.Log("Run Model: " + screenModelMap);
            await _semaphoreSlim.WaitAsync();

            try
            {
                await _sceneService.LoadScenesForModule(screenModelMap);

                var sceneLifetimeScope = _sceneInstallerManager.
                    CombineScenes(LifetimeScope.Find<RootLifetimeScope>());

                _currentModel = _screenTypeMapper.Resolve(screenModelMap, sceneLifetimeScope.Container);

                ModuleChanged?.Invoke(sceneLifetimeScope.Container);

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

        private static ScreenModelMap? SceneNameToEnum(string sceneName)
        {
            if (Enum.TryParse(sceneName, out ScreenModelMap result))
                return result;
            return null;
        }
    }
}
