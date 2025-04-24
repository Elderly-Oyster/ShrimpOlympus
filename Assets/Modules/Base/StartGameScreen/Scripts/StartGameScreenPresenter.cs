using System;
using System.Threading;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Patterns.Architecture.MVP;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Modules.Base.StartGameScreen.Scripts
{
    public class StartGameScreenPresenter : IModuleController
    {
        private const int TooltipDelay = 3000;
        private const int AppFrameRate = 60;
       
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly StartGameScreenModel _startGameScreenModel;
        private readonly StartGameView _startGameView;
        private readonly IScreenStateMachine _screenStateMachine;
        
        private readonly ReactiveProperty<string> _progressStatus = new(string.Empty);
        private readonly ReactiveProperty<float> _exponentialProgress = new(0f);
        private readonly ReactiveCommand<Unit> _startCommand = new();
        
        public ReadOnlyReactiveProperty<string> ProgressStatus => 
            _progressStatus.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<float> ExponentialProgress => 
            _exponentialProgress.ToReadOnlyReactiveProperty();
        public ReactiveCommand<Unit> StartCommand => _startCommand;
        
        public StartGameScreenPresenter(IScreenStateMachine screenStateMachine,
            StartGameScreenModel startGameScreenModel, StartGameView startGameView)
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            
            _screenStateMachine = screenStateMachine;
            _startGameScreenModel = startGameScreenModel;
            _startGameView = startGameView;

            SubscribeToUIUpdates();
        }
        
        private void SubscribeToUIUpdates()
        {
            _startCommand.Subscribe(_ => OnContinueButtonPressed())
                .AddTo(_cancellationTokenSource.Token);
        }
        
        public async UniTask Enter(object param)
        {
            SetVersionText(Application.version);
            SetApplicationFrameRate();
            InitializeUI();
    
            ShowTooltips().Forget();
            _startGameScreenModel.DoTweenInit();
            _startGameScreenModel.RegisterCommands();

            await _startGameView.Show();

            await InitializeServices();

            ShowAnimations();
        }
        
        private void InitializeUI()
        {
            _startGameView.HideInstantly();
            _startGameView.SetupEventListeners(StartCommand,
                ProgressStatus, ExponentialProgress); 
        }

        private async UniTask InitializeServices()
        {
            var timing = 1f / _startGameScreenModel.Commands.Count;
            var currentTiming = timing;
            
            foreach (var (serviceName, initFunction) in _startGameScreenModel.Commands)
            {
                _progressStatus.Value = $"Loading: {serviceName}";
                _exponentialProgress.Value = CalculateExponentialProgress(currentTiming);
                currentTiming += timing;
                
                await initFunction.Invoke().AsUniTask();
            }
        }

        private static float CalculateExponentialProgress(float progress)
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
            await _startGameView.Hide();
        }

        private static void SetApplicationFrameRate() => 
            Application.targetFrameRate = AppFrameRate;

        private void RunMainMenuScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }

        private void OnContinueButtonPressed() => RunMainMenuScreen(ScreenPresenterMap.MainMenu);

        private void SetVersionText(string appVersion) => _startGameView.SetVersionText(appVersion);

        private void ShowAnimations() => _startGameView.ShowAnimations(_cancellationTokenSource.Token);

        private async UniTaskVoid ShowTooltips()
        {
            var token = _cancellationTokenSource.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var tooltip = _startGameScreenModel.GetNextTooltip();
                    _startGameView.SetTooltipText(tooltip);
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
            
            _startGameView.Dispose();
            _startGameScreenModel.Dispose();
        }
    }
}
