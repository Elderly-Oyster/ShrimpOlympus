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
        private readonly StartGameScreenModel _startGameScreenModel;
        private readonly StartGameScreenView _startGameScreenView;
        private readonly IRootController _rootController;

        private readonly FirstLongInitializationService _firstLongInitializationService;
        private readonly SecondLongInitializationService _secondLongInitializationService;
        private readonly ThirdLongInitializationService _thirdLongInitializationService;

        public StartGameScreenPresenter(IRootController rootController,
            StartGameScreenModel startGameScreenModel, StartGameScreenView startGameScreenView,
            FirstLongInitializationService firstLongInitializationService,
            SecondLongInitializationService secondLongInitializationService,
            ThirdLongInitializationService thirdLongInitializationService)
        {
            _firstLongInitializationService = firstLongInitializationService;
            _secondLongInitializationService = secondLongInitializationService;
            _thirdLongInitializationService = thirdLongInitializationService;

            _startGameScreenModel = startGameScreenModel;
            _startGameScreenView = startGameScreenView;
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
            _startGameScreenView.SetVersionText(StartGameScreenModel.appVersion);
            DoTweenInit();
            RegisterCommands();
            
            var timing = 1f / _commands.Count;
            var currentTiming = timing;
            
            foreach (var (serviceName, initFunction) in _commands)
            {
                await Task.WhenAll(initFunction.Invoke(), UpdateViewWithModelData(currentTiming, serviceName).AsTask());
                currentTiming += timing;
            }
            
            using (var cts = new CancellationTokenSource())
            {
                _startGameScreenView.ShowAnimations(cts.Token);
                await _startGameScreenView.WaitButton();
                cts.Cancel(); // Крутая штука, если нужно явно отменить задачи перед выходом из блока using
            }
            _rootController.RunPresenter(ScreenPresenterMap.Converter);
        } // Не нужно вызывать cts.Dispose(), блок using делает это автоматически

        private UniTask UpdateViewWithModelData(float progress, string serviceName)
        {
            _startGameScreenModel.UpdateProgress(progress, serviceName);
            return _startGameScreenView.
                ReportProgress(_startGameScreenModel.exponentialProgress, _startGameScreenModel.progressStatus);
        }

        public async UniTask Stop() => await _startGameScreenView.Hide();

        public void Dispose() => _startGameScreenView.Dispose();
    }
}