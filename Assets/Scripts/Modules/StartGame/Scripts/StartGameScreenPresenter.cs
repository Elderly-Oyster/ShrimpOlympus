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
    public class StartGameScreenPresenter : IScreenPresenter
    {
        private readonly Dictionary<string, Func<Task>> _commands;
        private readonly StartGameUIView _startGameUIView;
        private readonly IRootController _rootController;

        private readonly FirstLongInitializationService _firstLongInitializationService;
        private readonly SecondLongInitializationService _secondLongInitializationService;
        private readonly ThirdLongInitializationService _thirdLongInitializationService;

        public StartGameScreenPresenter(StartGameUIView startGameUIView, IRootController rootController,
            FirstLongInitializationService firstLongInitializationService,
            SecondLongInitializationService secondLongInitializationService,
            ThirdLongInitializationService thirdLongInitializationService)
        {
            _firstLongInitializationService = firstLongInitializationService;
            _secondLongInitializationService = secondLongInitializationService;
            _thirdLongInitializationService = thirdLongInitializationService;
            
            _startGameUIView = startGameUIView;
            _rootController = rootController;

            _commands = new Dictionary<string, Func<Task>>();
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
            DoTweenInit();
            RegisterCommands();
            
            var timing = 1f / _commands.Count;
            var currentTiming = timing;
            foreach (var (key, value) in _commands)
            {
                await Task.WhenAll(value.Invoke(), _startGameUIView.ReportProgress(currentTiming, key).AsTask());
                currentTiming += timing;
            }
            
            var cts = new CancellationTokenSource();
            _startGameUIView.ShowAnimations(cts.Token);
            await _startGameUIView.WaitButton();
            cts.Cancel();
            cts.Dispose();
            _rootController.RunPresenter(ScreenPresenterMap.Converter);
        }

        public async UniTask Stop() => await _startGameUIView.Hide();

        public void Dispose() => _startGameUIView.Dispose();
    }
}