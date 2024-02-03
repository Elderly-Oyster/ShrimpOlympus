using System;
using UnityEngine;

namespace Modules.StartGame.Scripts
{
    public class StartGameScreenModel
    {
        public float exponentialProgress { get; private set; }
        public string progressStatus { get; private set; }
        
        public static string appVersion => Application.version;
        
        private readonly string[] _tooltips;
        private int _currentTooltipIndex;
        
        public StartGameScreenModel()
        {
            _tooltips = new []
            {
                "Quickly check live exchange rates for over 1 currencies!",
                "Simple and easy-to-use interface to convert currencies on-the-go!",
                "Traveling abroad? Get accurate currency conversions instantly!",
                "Make informed financial decisions with precise currency conversion at your fingertips!",
                "Your reliable companion for international shopping, travel, and business!"
            };
        }
        
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
        
        public string GetNextTooltip()
        {
            var tooltip = _tooltips[_currentTooltipIndex];
            _currentTooltipIndex = (_currentTooltipIndex + 1) % _tooltips.Length;
            return tooltip;
        }
    }
}