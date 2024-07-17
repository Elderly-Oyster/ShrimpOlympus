using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Services
{
    public class AudioListenerService : IStartable
    {
        [Inject] private ScreenController _screenController;

        public void Start() => _screenController.ModuleChanged += OnModuleChanged;

        private void OnModuleChanged(IObjectResolver resolver) => EnsureAudioListenerExists(resolver);

        private void EnsureAudioListenerExists(IObjectResolver resolver)
        {
            var mainCamera = resolver.Resolve<Camera>();
            if (mainCamera == null)
            {
                mainCamera = new GameObject("MainCamera").AddComponent<Camera>();
                resolver.Inject(mainCamera);
            }

            var audioListener = mainCamera.GetComponent<AudioListener>();
            if (audioListener == null)
                mainCamera.gameObject.AddComponent<AudioListener>();
        }
    }
}