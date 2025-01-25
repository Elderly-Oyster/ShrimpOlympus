using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Core.Modules;
using CodeBase.Services.LongInitializationServices;
using DG.Tweening;
using DG.Tweening.Core.Enums;

namespace Modules.Base.StartGameScreen.Scripts
{
    public class StartGameScreenModel : IScreenModel
    {
        public readonly Dictionary<string, Func<Task>> Commands;

        private readonly FirstLongInitializationService _firstLongInitializationService;
        private readonly SecondLongInitializationService _secondLongInitializationService;
        private readonly ThirdLongInitializationService _thirdLongInitializationService;
        
        private readonly string[] _tooltips;
        private int _currentTooltipIndex;

        public StartGameScreenModel(FirstLongInitializationService firstLongInitializationService,
            SecondLongInitializationService secondLongInitializationService,
            ThirdLongInitializationService thirdLongInitializationService)
        {
            _firstLongInitializationService = firstLongInitializationService;
            _secondLongInitializationService = secondLongInitializationService;
            _thirdLongInitializationService = thirdLongInitializationService;

            Commands = new Dictionary<string, Func<Task>>();
            
            _tooltips = new []
            {
                "Quickly check live exchange rates for over 1 currencies!",
                "Simple and easy-to-use interface to convert currencies on-the-go!",
                "Traveling abroad? Get accurate currency conversions instantly!",
                "Make informed financial decisions with precise currency conversion at your fingertips!",
                "Your reliable companion for international shopping, travel, and business!"
            };
        }
        
        public void DoTweenInit()
        {
            DOTween.Init().SetCapacity(240, 30);
            DOTween.safeModeLogBehaviour = SafeModeLogBehaviour.None;
            DOTween.defaultAutoKill = true;
            DOTween.defaultRecyclable = true;
            DOTween.useSmoothDeltaTime = true;
        }

        public void RegisterCommands()
        {
            Commands.Add("First Service", _firstLongInitializationService.Init);
            Commands.Add("Second Service", _secondLongInitializationService.Init);
            Commands.Add("Third Service", _thirdLongInitializationService.Init);
        }
        
        public string GetNextTooltip()
        {
            var tooltip = _tooltips[_currentTooltipIndex];
            _currentTooltipIndex = (_currentTooltipIndex + 1) % _tooltips.Length;
            return tooltip;
        }

        public void Dispose() => Commands.Clear();
    }
}
