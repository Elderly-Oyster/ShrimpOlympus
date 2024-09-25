using System;
using System.Collections.Generic;
using System.Threading;
using Core.MVP;
using Core.Root.ScreenStateMachine;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Modules.Additional.LoadingSplashScreen.Scripts
{
    public class LoadingSplashScreenPresenter : IScreenPresenter
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly LoadingSplashScreenModel _loadingSplashScreenModel;
        private readonly LoadingSplashScreenView _loadingSplashScreenView;
        private readonly IScreenStateMachine _screenStateMachine;
        
        private readonly ReactiveProperty<string> _progressStatus = new(string.Empty);
        private readonly ReactiveProperty<float> _exponentialProgress = new(0f);
        private readonly ReactiveCommand<Unit> _startCommand = new();

        private const int TooltipDelay = 3000;
        private const int AppFrameRate = 60;
        
        public LoadingSplashScreenPresenter(IScreenStateMachine screenStateMachine,
            LoadingSplashScreenModel loadingSplashScreenModel, LoadingSplashScreenView loadingSplashScreenView)
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            
            _screenStateMachine = screenStateMachine;
            _loadingSplashScreenModel = loadingSplashScreenModel;
            _loadingSplashScreenView = loadingSplashScreenView;

            SubscribeToUIUpdates();
        }
        
        private void SubscribeToUIUpdates()
        {
            _startCommand.Subscribe(_ => OnContinueButtonPressed())
                .AddTo(_cancellationTokenSource.Token);

            _exponentialProgress.Subscribe(progress =>
            {
                _loadingSplashScreenView.ReportProgress(progress,
                    _progressStatus.Value).Forget();
            }).AddTo(_cancellationTokenSource.Token);

            _progressStatus.Subscribe(status => 
                _loadingSplashScreenView.SetTooltipText(status))
                .AddTo(_cancellationTokenSource.Token);
        }

        public async UniTask Enter(object param)
        {
            SetApplicationFrameRate();
            InitializeUI();
    
            ShowTooltips().Forget();
            _loadingSplashScreenModel.DoTweenInit();
            _loadingSplashScreenModel.RegisterCommands();

            await _loadingSplashScreenView.Show();

            // await ShowScenesLoadingProgress();
            // await InitializeServices();  TODO

            ShowAnimations();
        }
        
        private void InitializeUI() => _loadingSplashScreenView.HideInstantly();

        private async UniTask ShowScenesLoadingProgress(List<UniTask> loadSceneTasks)
        {
            var timing = 1f / loadSceneTasks.Count;
            var currentTiming = timing;
            
            foreach (var initFunction in loadSceneTasks)
            {
                _exponentialProgress.Value = CalculateExponentialProgress(currentTiming);

                loadSceneTasks.Add(initFunction);
                currentTiming += timing;
            }

            await UniTask.WhenAll(loadSceneTasks);
            
        }
        // private async UniTask ShowScenesLoadingProgress(List<UniTask> loadSceneTasks)
        // {
        //     var timing = 1f / loadSceneTasks.Count;
        //     var currentTiming = timing;
        //     
        //     foreach (var (serviceName, initFunction) in _loadingSplashScreenModel.Commands)
        //     {
        //         var initTask = initFunction.Invoke().AsUniTask();
        //
        //         _progressStatus.Value = $"Loading: {serviceName}";
        //         _exponentialProgress.Value = CalculateExponentialProgress(currentTiming);
        //
        //         loadSceneTasks.Add(initTask);
        //         currentTiming += timing;
        //     }
        //
        //     await UniTask.WhenAll(loadSceneTasks);
        // }
        
        // private async UniTask InitializeServices()
        // {
        //     var timing = 1f / _loadingSplashScreenModel.Commands.Count;
        //     var currentTiming = timing;
        //
        //     var initTasks = new List<UniTask>();
        //
        //     foreach (var (serviceName, initFunction) in _loadingSplashScreenModel.Commands)
        //     {
        //         var initTask = initFunction.Invoke().AsUniTask();
        //
        //         _progressStatus.Value = $"Loading: {serviceName}";
        //         _exponentialProgress.Value = CalculateExponentialProgress(currentTiming);
        //
        //         initTasks.Add(initTask);
        //         currentTiming += timing;
        //     }
        //
        //     await UniTask.WhenAll(initTasks);
        // }

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
            await _loadingSplashScreenView.Hide();
        }

        private void SetApplicationFrameRate() => Application.targetFrameRate = AppFrameRate;

        private void RunMainMenuScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }

        private void OnContinueButtonPressed() => RunMainMenuScreen(ScreenPresenterMap.MainMenu);
        
        private void ShowAnimations() => _loadingSplashScreenView.ShowAnimations(_cancellationTokenSource.Token);

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
