using CodeBase.Core.Infrastructure;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Core.Systems.Save;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Base.DeliveryTycoon.Scripts.PausePopup
{
    public class PausePresenter : IPresenter
    {
        private readonly GameModel _gameModel;
        private readonly PauseView _pauseView;
        private readonly SaveSystem _saveSystem;
        private readonly ReactiveCommand<Unit> _onMainMenuButtonClickedCommand = new();
        private UniTaskCompletionSource<bool> _screenCompletionSource;
        private CompositeDisposable _disposables = new();

        public ReactiveCommand<Unit> ExitPauseCommand { get; private set; } = new();
        public ReactiveCommand<ModulesMap> OpenNewModuleCommand { get; private set; } = new();

        public PausePresenter(GameModel gameModel, PauseView pauseView,
            SaveSystem saveSystem)
        {
            _gameModel = gameModel;
            _pauseView = pauseView;
            _saveSystem = saveSystem;
            
            
            SubscribeToUICommands();
            _pauseView.SetupEventListeners
            (
                OpenNewModuleCommand,
                ExitPauseCommand
                );
        }

        public async UniTask Enter(object param)
        {
            _screenCompletionSource = new UniTaskCompletionSource<bool>();
            _pauseView.HideInstantly();
            await ShowState();
        }
        
        private void SubscribeToUICommands()
        {
            ExitPauseCommand
                .Subscribe(async _ => await OnExitPauseCommand())
                .AddTo(_disposables);

            //other commands
        }

        public async UniTask Exit()
        {
            if (_pauseView.isActiveAndEnabled)
            {
                await HideState();
                _screenCompletionSource.TrySetResult(true);
            }
        }

        public async UniTask ShowState()
        {
            await _pauseView.Show();
            //other hide sub-state logic
        }

        public async UniTask HideState()
        {
            await _pauseView.Hide();
            //other hide sub-state logic
        }
        
        public void HideStateInstantly() => _pauseView.HideInstantly();

        private async UniTask OnExitPauseCommand() => await _gameModel.ChangeState(GameModuleStates.Game);

        public void Dispose()
        {
            _pauseView.Dispose();
            _gameModel.Dispose();
            _disposables.Dispose();
        }
    }
}