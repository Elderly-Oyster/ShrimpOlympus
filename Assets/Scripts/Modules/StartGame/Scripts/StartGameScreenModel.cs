using System;
using UnityEngine;

namespace Modules.StartGame.Scripts
{
    public class StartGameScreenModel
    {
        public static string appVersion => Application.version;
        public float exponentialProgress { get; private set; }
        public string progressStatus { get; private set; }
        
        public void UpdateProgress(float progress, string serviceName)
        {
            progressStatus = $"Loading: {serviceName}";
            exponentialProgress = CalculateExponentialProgress(progress);
        }
        
        private float CalculateExponentialProgress(float progress)
        {
            var expValue = Math.Exp(progress);
            var minExp = Math.Exp(0);
            var maxExp = Math.Exp(1);
            return (float)((expValue - minExp) / (maxExp - minExp));
        }
    }
}