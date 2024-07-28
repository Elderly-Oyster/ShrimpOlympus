using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Modules.StartGameScreen.Scripts
{
    public class StartGameScreenPresenter : IPresenter
    {
        [Inject] private readonly StartGameScreenView _startGameScreenView;
        private readonly UniTaskCompletionSource<bool> _continueButtonPressed = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private const int TooltipDelay = 3000;
        private float exponentialProgress { get; set; }
        private string progressStatus { get; set; }
        private StartGameScreenModel _startGameScreenModel; //Без инжекта, т.к. появлялась Circle Dependency Exception

        private static string appVersion;
        private static int appFrameRate = 60;

        public void Initialize(StartGameScreenModel startGameScreenModel)
        {
            _startGameScreenModel = startGameScreenModel;
            SetApplicationFrameRate();
            _startGameScreenView.SetupEventListeners(OnContinueButtonPressed);
        }

        private void OnContinueButtonPressed()
        {
            _startGameScreenModel.RunMainMenuModel();
            _continueButtonPressed.TrySetResult(true);
        } 

        public async UniTask WaitForContinueButtonPress() => await _continueButtonPressed.Task;

        public void SetVersionText(string appVersion) => _startGameScreenView.SetVersionText(appVersion);

        public void ShowAnimations() => _startGameScreenView.ShowAnimations(_cancellationTokenSource.Token);

        public async UniTaskVoid ShowTooltips()
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
        
        public UniTask UpdateViewWithModelData(float progress, string serviceName)
        {
            UpdateProgress(progress, serviceName);
            return _startGameScreenView.
                ReportProgress(exponentialProgress, progressStatus);
        }

        public async UniTask HideScreenView()
        {
            await _startGameScreenView.Hide();
            _startGameScreenView.RemoveEventListeners();
            _startGameScreenView.Dispose();
            _cancellationTokenSource.Cancel();

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

        private void SetApplicationFrameRate() => Application.targetFrameRate = appFrameRate;
        public string GetAppVersion() => Application.version;

        public void RemoveEventListeners() => _startGameScreenView.RemoveEventListeners();
    }
}