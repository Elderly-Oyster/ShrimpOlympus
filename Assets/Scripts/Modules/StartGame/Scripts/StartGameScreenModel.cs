using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core.Enums;
using Services;
using UnityEngine;

namespace Modules.StartGame.Scripts
{
    public class StartGameScreenModel : IScreenModel
    {
        private readonly Dictionary<string, Func<Task>> _commands;
        private readonly StartGameScreenPresenter _startGameScreenPresenter;
        private readonly IRootController _rootController;

        private readonly FirstLongInitializationService _firstLongInitializationService;
        private readonly SecondLongInitializationService _secondLongInitializationService;
        private readonly ThirdLongInitializationService _thirdLongInitializationService;

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        private readonly string[] _tooltips;
        private int _currentTooltipIndex;
        private static string appVersion => Application.version;

        public StartGameScreenModel(IRootController rootController, StartGameScreenPresenter startGameScreenPresenter,
            FirstLongInitializationService firstLongInitializationService,
            SecondLongInitializationService secondLongInitializationService,
            ThirdLongInitializationService thirdLongInitializationService)
        {
            _firstLongInitializationService = firstLongInitializationService;
            _secondLongInitializationService = secondLongInitializationService;
            _thirdLongInitializationService = thirdLongInitializationService;

            _startGameScreenPresenter = startGameScreenPresenter;
            _rootController = rootController;

            _commands = new Dictionary<string, Func<Task>>();
            
            _tooltips = new []
            {
                "Quickly check live exchange rates for over 1 currencies!",
                "Simple and easy-to-use interface to convert currencies on-the-go!",
                "Traveling abroad? Get accurate currency conversions instantly!",
                "Make informed financial decisions with precise currency conversion at your fingertips!",
                "Your reliable companion for international shopping, travel, and business!"
            };
        }
        
        private void DoTweenInit()
        {
            DOTween.Init().SetCapacity(240, 30);
            DOTween.safeModeLogBehaviour = SafeModeLogBehaviour.None;
            DOTween.defaultAutoKill = true;
            DOTween.defaultRecyclable = true;
            DOTween.useSmoothDeltaTime = true;
        }

        private void RegisterCommands()
        {
            _commands.Add("First Service", _firstLongInitializationService.Init);
            _commands.Add("Second Service", _secondLongInitializationService.Init);
            _commands.Add("Third Service", _thirdLongInitializationService.Init);
        }

        public async UniTask Run(object param)
        {
            Application.targetFrameRate = 60;
            _startGameScreenPresenter.Initialize();
            _startGameScreenPresenter.SetVersionText(appVersion);
            _startGameScreenPresenter.ShowTooltips(_cancellationTokenSource.Token).Forget();
            DoTweenInit();
            RegisterCommands();

            var timing = 1f / _commands.Count;
            var currentTiming = timing;

            foreach (var (serviceName, initFunction) in _commands)
            {
                await Task.WhenAll(initFunction.Invoke(), _startGameScreenPresenter.UpdateViewWithModelData(currentTiming, serviceName).AsTask());
                currentTiming += timing;
            }
            
            _startGameScreenPresenter.ShowAnimations(_cancellationTokenSource.Token);
        }
        
        
        public string GetNextTooltip()
        {
            var tooltip = _tooltips[_currentTooltipIndex];
            _currentTooltipIndex = (_currentTooltipIndex + 1) % _tooltips.Length;
            return tooltip;
        }
        
        public void RunConverterModel() => RootControllerExtension.RunModel(_rootController, ScreenModelMap.Converter);

        public async UniTask Stop()
        {
            await _startGameScreenPresenter.HideScreenView();
        }
        
        public void Dispose()
        {
            _startGameScreenPresenter.RemoveEventListeners();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        } 
    }
}