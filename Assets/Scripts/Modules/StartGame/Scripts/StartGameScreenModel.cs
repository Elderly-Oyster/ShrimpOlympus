using UnityEngine;

namespace Modules.StartGame.Scripts
{
    public class StartGameScreenModel
    {
        public string appVersion => Application.version; 
        public float currentProgress { get; private set; }
        public string currentServiceName { get; private set; }

        public void UpdateProgress(float progress, string serviceName)
        {
            currentProgress = progress;
            currentServiceName = serviceName;
        }
    }
}