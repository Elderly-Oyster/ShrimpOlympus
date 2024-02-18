using System;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Modules.StartGame.Scripts
{
    public class StartGameScreenPresenter : IPresenter
    {
        [Inject] private readonly StartGameScreenView _startGameScreenView;
        private const int TooltipDelay = 3000;
        private float exponentialProgress { get; set; }
        private string progressStatus { get; set; }
        public StartGameScreenModel startGameScreenModel { get; set; }

        public void Initialize()
        {
            _startGameScreenView.SetupEventListeners(startGameScreenModel.RunConverterModel);
        }

        public void SetVersionText(string appVersion) => _startGameScreenView.SetVersionText(appVersion);

        public void ShowAnimations(CancellationToken token) => _startGameScreenView.ShowAnimations(token);

        public async UniTaskVoid ShowTooltips(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var tooltip = startGameScreenModel.GetNextTooltip();
                    _startGameScreenView.SetTooltipText(tooltip);
                    await UniTask.Delay(TooltipDelay, cancellationToken: cancellationToken);
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

        public void RemoveEventListeners() => _startGameScreenView.RemoveEventListeners();
    }
}