using System;
using System.Collections.Generic;
using System.Threading;
using Core;
using Core.MVP;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Modules.Base.StartGameScreen.Scripts
{
    public class StartGameScreenPresenter : IScreenPresenter
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly StartGameScreenModel _startGameScreenModel;
        private readonly StartGameScreenView _startGameScreenView;
        private readonly IScreenStateMachine _screenStateMachine;
        
        private readonly ReactiveProperty<string> _progressStatus = new(string.Empty);
        private readonly ReactiveProperty<float> _exponentialProgress = new(0f);
        private readonly ReactiveCommand _startCommand = new ReactiveCommand();

        private const int TooltipDelay = 3000;
        private const int AppFrameRate = 60;

        
        public StartGameScreenPresenter(IScreenStateMachine screenStateMachine,
            StartGameScreenModel startGameScreenModel, StartGameScreenView startGameScreenView)
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            
            _screenStateMachine = screenStateMachine;
            _startGameScreenModel = startGameScreenModel;
            _startGameScreenView = startGameScreenView;

            SubscribeToUIUpdates();
        }
        
        private void SubscribeToUIUpdates()
        {
            _startCommand.Subscribe(_ => OnContinueButtonPressed())
                .AddTo(_cancellationTokenSource.Token);

            _exponentialProgress.Subscribe(progress =>
            {
                _startGameScreenView.ReportProgress(progress,
                    _progressStatus.Value).Forget();
            }).AddTo(_cancellationTokenSource.Token);

            _progressStatus.Subscribe(status => 
                _startGameScreenView.SetTooltipText(status))
                .AddTo(_cancellationTokenSource.Token);
        }
        
        public async UniTask Enter(object param)
        {
            SetApplicationFrameRate();
            InitializeUI();
    
            ShowTooltips().Forget();
            _startGameScreenModel.DoTweenInit();
            _startGameScreenModel.RegisterCommands();

            await _startGameScreenView.Show();

            await InitializeServices();

            ShowAnimations();
        }
        
        private void InitializeUI()
        {
            _startGameScreenView.HideInstantly();
            _startGameScreenView.SetupEventListeners(_startCommand);
            SetVersionText(Application.version);
        }

        private async UniTask InitializeServices()
        {
            var timing = 1f / _startGameScreenModel.Commands.Count;
            var currentTiming = timing;

            var initTasks = new List<UniTask>();

            foreach (var (serviceName, initFunction) in _startGameScreenModel.Commands)
            {
                var initTask = initFunction.Invoke().AsUniTask();

                _progressStatus.Value = $"Loading: {serviceName}";
                _exponentialProgress.Value = CalculateExponentialProgress(currentTiming);

                initTasks.Add(initTask);
                currentTiming += timing;
            }

            await UniTask.WhenAll(initTasks);
        }

        private float CalculateExponentialProgress(float progress)
        {
            var expValue = Math.Exp(progress);
            var minExp = Math.Exp(0);
            var maxExp = Math.Exp(1);
            return (float)((expValue - minExp) / (maxExp - minExp));
        }



        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit()
        {
            _cancellationTokenSource?.Cancel();
            await _startGameScreenView.Hide();
        }

        private void SetApplicationFrameRate() => Application.targetFrameRate = AppFrameRate;

        private void RunMainMenuScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunPresenter(screen);
        }

        private void OnContinueButtonPressed() => RunMainMenuScreen(ScreenPresenterMap.MainMenu);

        private void SetVersionText(string appVersion) => _startGameScreenView.SetVersionText(appVersion);

        private void ShowAnimations() => _startGameScreenView.ShowAnimations(_cancellationTokenSource.Token);

        private async UniTaskVoid ShowTooltips()
        {
            var token = _cancellationTokenSource.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var tooltip = _startGameScreenModel.GetNextTooltip();
                    _startGameScreenView.SetTooltipText(tooltip);
                    await UniTask.Delay(TooltipDelay, cancellationToken: token);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Debug.LogError($"ShowTooltips Error: {ex.Message}"); }
        }

        public void Dispose()
        {
            if (_cancellationTokenSource is {IsCancellationRequested: false}) 
                _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
            
            _startGameScreenView.Dispose();
            _startGameScreenModel.Dispose();
        }
    }
}
