using Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Services
{
    public class AudioListenerService : IStartable
    {
        [Inject] private ScreenStateMachine _screenStateMachine;

        public void Start() => _screenStateMachine.ModuleChanged += OnModuleChanged;

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