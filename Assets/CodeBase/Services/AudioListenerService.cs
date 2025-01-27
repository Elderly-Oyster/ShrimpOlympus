using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Services
{
    public class AudioListenerService : IStartable
    {
        public void Start() { } // TODO Check The Need
        
        //TODO Рефакторинг, в том числе лииитттерала
        public void EnsureAudioListenerExists(IObjectResolver resolver)
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