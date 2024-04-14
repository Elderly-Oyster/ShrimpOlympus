using System;
using UnityEngine;

namespace Services.App
{
    public class ApplicationService : MonoBehaviour, IApplicationService
    {
        public event Action ApplicationPause;
        public event Action ApplicationResume;
        public event Action ApplicationFocus;
        public event Action ApplicationFocusLost;
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                ApplicationPause?.Invoke();
            else
                ApplicationResume?.Invoke();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
                ApplicationFocus?.Invoke();
            else
                ApplicationFocusLost?.Invoke();
        }
    }
}