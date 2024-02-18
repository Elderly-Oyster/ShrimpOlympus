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
    public class StartGameScreenPresenter : IPresenter
    {
        private readonly Dictionary<string, Func<Task>> _commands;
        private readonly StartGameScreenModel _startGameScreenModel;
        private readonly StartGameScreenView _startGameScreenView;
        private readonly IRootController _rootController;

        private readonly FirstLongInitializationService _firstLongInitializationService;
        private readonly SecondLongInitializationService _secondLongInitializationService;
        private readonly ThirdLongInitializationService _thirdLongInitializationService;

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private const int TooltipDelay = 3000;

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
            //TODO Вызвать не в ране, а тут отдельно
            _startGameScreenView.SetupEventListeners
            (
                RunConverterModel
            );
            
            Application.targetFrameRate = 60;
            _startGameScreenView.SetVersionText(StartGameScreenModel.appVersion);
            ShowTooltips(_cancellationTokenSource.Token).Forget();
            DoTweenInit();
            RegisterCommands();

            var timing = 1f / _commands.Count;
            var currentTiming = timing;

            foreach (var (serviceName, initFunction) in _commands)
            {
                await Task.WhenAll(initFunction.Invoke(), UpdateViewWithModelData(currentTiming, serviceName).AsTask());
                currentTiming += timing;
            }
            
            _startGameScreenView.ShowAnimations(_cancellationTokenSource.Token);
        } 
        
        private void RunConverterModel() => RootControllerExtension.RunModel(_rootController, ScreenModelMap.Converter);

        //TODO
        private async UniTaskVoid ShowTooltips(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var tooltip = _startGameScreenModel.GetNextTooltip();
                    _startGameScreenView.SetTooltipText(tooltip);
                    await UniTask.Delay(TooltipDelay, cancellationToken: cancellationToken);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Debug.LogError($"ShowTooltips Error: {ex.Message}"); }
        }
        //TODO
        private UniTask UpdateViewWithModelData(float progress, string serviceName)
        {
            _startGameScreenModel.UpdateProgress(progress, serviceName);
            return _startGameScreenView.
                ReportProgress(_startGameScreenModel.exponentialProgress, _startGameScreenModel.progressStatus);
        }
        //TODO Метод HideView
        public async void HideScreenView() => await _startGameScreenView.Hide();

        public void Dispose()
        {
            _startGameScreenView.RemoveEventListeners();
            _startGameScreenView.Dispose();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        } 
    }
}