using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Modules.Additional.LoadingSplashScreen.Scripts
{
    public class LoadingSplashScreenPresenter
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly LoadingSplashScreenModel _loadingSplashScreenModel;
        private readonly LoadingSplashScreenView _loadingSplashScreenView;
        
        private readonly ReactiveProperty<string> _progressStatus = new(string.Empty);
        private readonly ReactiveProperty<float> _exponentialProgress = new(0f);

        private const int TooltipDelay = 3000;

        //TODO Самое актуальное. Сначала ожидание метода сцен сервиса по подгрузке всех сцен, а потом
        //ожидания 
        public LoadingSplashScreenPresenter(LoadingSplashScreenModel loadingSplashScreenModel, LoadingSplashScreenView loadingSplashScreenView)
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            
            _loadingSplashScreenModel = loadingSplashScreenModel;
            _loadingSplashScreenView = loadingSplashScreenView;

            SubscribeToUIUpdates();
        }
        
        private void SubscribeToUIUpdates()
        {
            _exponentialProgress.Subscribe(progress =>
            {
                _loadingSplashScreenView.ReportProgress(progress,
                    _progressStatus.Value).Forget();
            }).AddTo(_cancellationTokenSource.Token);

            _progressStatus.Subscribe(status => 
                _loadingSplashScreenView.SetTooltipText(status))
                .AddTo(_cancellationTokenSource.Token);
        }

        public async UniTask Enter()
        {
            InitializeUI();
    
            ShowTooltips().Forget();
            
            _loadingSplashScreenModel.RegisterCommands();

            await _loadingSplashScreenView.Show();
        }

        public async UniTask DisplayLoading(Action scenesLoadProgress, Action servicesLoadProgress)
        {
            await WaitScenesInitialization(scenesLoadProgress);
            await WaitServicesInitialization(servicesLoadProgress);

            await _completionSource.Task;
        }

        private void InitializeUI() => _loadingSplashScreenView.HideInstantly();

        //Action - задел на будущее, потом он будет возращать float progress
        private async UniTask WaitScenesInitialization(Action loadAction)
        {
            UniTaskCompletionSource completionSource = new();

            loadAction += () => completionSource.TrySetResult();

            await completionSource.Task;
        }

        private async UniTask WaitServicesInitialization(Action loadAction)
        {
            UniTaskCompletionSource completionSource = new();

            loadAction += () => completionSource.TrySetResult();

            await completionSource.Task;
            
            // // var timing = 1f / _loadingSplashScreenModel.Commands.Count;
            // var currentTiming = timing;
            //
            // var initTasks = new List<UniTask>();
            //
            // foreach (var (serviceName, initFunction) in _loadingSplashScreenModel.Commands)
            // {
            //     var initTask = initFunction.Invoke().AsUniTask();
            //
            //     _progressStatus.Value = $"Loading: {serviceName}";
            //     _exponentialProgress.Value = CalculateExponentialProgress(currentTiming);
            //
            //     initTasks.Add(initTask);
            //     currentTiming += timing;
            // }
            //
            // await UniTask.WhenAll(initTasks);
        }

        public async UniTask Hide()
        {
            _cancellationTokenSource?.Cancel();
            await _loadingSplashScreenView.Hide();
        }

        private float CalculateExponentialProgress(float progress)
        {
            var expValue = Math.Exp(progress);
            var minExp = Math.Exp(0);
            var maxExp = Math.Exp(1);
            return (float)((expValue - minExp) / (maxExp - minExp));
        }

        private async UniTaskVoid ShowTooltips()
        {
            var token = _cancellationTokenSource.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var tooltip = _loadingSplashScreenModel.GetNextTooltip();
                    _loadingSplashScreenView.SetTooltipText(tooltip);
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
            
            _loadingSplashScreenView.Dispose();
            _loadingSplashScreenModel.Dispose();
        }
    }
}
