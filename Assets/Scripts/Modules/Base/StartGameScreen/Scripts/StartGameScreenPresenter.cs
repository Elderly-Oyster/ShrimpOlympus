using System;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.MVVM;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.Base.StartGameScreen.Scripts
{
    public class StartGameScreenPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly StartGameScreenView _startGameScreenView;
        private readonly StartGameScreenModel _startGameScreenModel;
        private readonly UniTaskCompletionSource<bool> _completionSource;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private const int TooltipDelay = 3000;
        private float exponentialProgress { get; set; }
        private string progressStatus { get; set; }
        private static string _appVersion;
        private const int AppFrameRate = 60;

        public StartGameScreenPresenter(IScreenStateMachine screenStateMachine, StartGameScreenModel gameScreenModel,
            StartGameScreenView startGameScreenView)
        {
            _screenStateMachine = screenStateMachine;
            _startGameScreenModel = gameScreenModel;
            _startGameScreenView = startGameScreenView;
            _completionSource = new UniTaskCompletionSource<bool>();
        }

        public async UniTask Enter(object param)
        {
            SetApplicationFrameRate();
            _startGameScreenView.gameObject.SetActive(false);
            _startGameScreenView.SetupEventListeners(OnContinueButtonPressed);
            
            SetVersionText(GetAppVersion());
            ShowTooltips().Forget();
            _startGameScreenModel.DoTweenInit();
            _startGameScreenModel.RegisterCommands();
            await _startGameScreenView.Show();

            var timing = 1f / _startGameScreenModel.Commands.Count;
            var currentTiming = timing;

            foreach (var (serviceName, initFunction) in _startGameScreenModel.Commands)
            {
                await Task.WhenAll(initFunction.Invoke(), 
                    UpdateViewWithModelData(currentTiming, serviceName).AsTask());
                currentTiming += timing;
            }
            ShowAnimations();
        }
        
        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit()
        {
            _cancellationTokenSource?.Cancel();
            await _startGameScreenView.Hide();
        }

        private void SetApplicationFrameRate() => Application.targetFrameRate = AppFrameRate;

        private string GetAppVersion() => Application.version;
        
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

        private UniTask UpdateViewWithModelData(float progress, string serviceName)
        {
            UpdateProgress(progress, serviceName);
            return _startGameScreenView.
                ReportProgress(exponentialProgress, progressStatus);
        }


        private void UpdateProgress(float progress, string serviceName)
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
        
        public void Dispose()
        {
            _startGameScreenView.Dispose();
            _startGameScreenModel.Dispose();
            _cancellationTokenSource?.Dispose();
        }
    }
}