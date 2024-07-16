using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Services
{
    public class AudioListenerService : IStartable
    {
        [Inject] private ScreenController _screenController;

        public void Start()
        {
            EnsureAudioListenerExists();
            _screenController.ModuleChanged += OnModuleChanged;
        }

        private void OnModuleChanged()
        {
            EnsureAudioListenerExists();
        }

        private void EnsureAudioListenerExists()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = new GameObject("MainCamera").AddComponent<Camera>();

            var audioListener = mainCamera.GetComponent<AudioListener>();
            if (audioListener == null)
                audioListener = mainCamera.gameObject.AddComponent<AudioListener>();

            Object.DontDestroyOnLoad(audioListener.gameObject);
        }
    }
}