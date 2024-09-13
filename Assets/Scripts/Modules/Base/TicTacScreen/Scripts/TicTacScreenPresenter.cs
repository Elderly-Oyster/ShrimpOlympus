using Core;
using Core.MVVM;
using Core.Popup.Base;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Modules.Base.TicTacScreen.Scripts
{
    public class TicTacScreenPresenter : IScreenPresenter
    {
        private readonly TicTacScreenView _ticTacScreenView;
        
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly TicTacScreenModel _newModuleScreenModel;
        private readonly TicTacScreenView _newModuleScreenView;
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly TicTacScreenModel _ticTacScreenModel;
        private readonly PopupHub _popupHub;

        private readonly ReactiveCommand _mainMenuCommand = new ReactiveCommand();
        private readonly ReactiveCommand<int[]> _cellCommand = new ReactiveCommand<int[]>();
        private readonly ReactiveCommand _restartCommand = new ReactiveCommand();
        private readonly ReactiveCommand _thirdPopupCommand = new ReactiveCommand();

        public TicTacScreenPresenter(IScreenStateMachine screenStateMachine, TicTacScreenModel newModuleScreenModel, TicTacScreenView newModuleScreenView, TicTacScreenView ticTacScreenView, TicTacScreenModel ticTacScreenModel, PopupHub popupHub)
        {
            _screenStateMachine = screenStateMachine;
            _newModuleScreenModel = newModuleScreenModel;
            _newModuleScreenView = newModuleScreenView;
            _ticTacScreenView = ticTacScreenView;
            _ticTacScreenModel = ticTacScreenModel;
            _popupHub = popupHub;
            _completionSource = new UniTaskCompletionSource<bool>();
        }

        private void SubscribeToUIUpdates()
        {
            _mainMenuCommand.Subscribe(_ => OnMainMenuButtonClicked());
            _cellCommand.Subscribe(position => OnCellClicked(position[0], position[1]));
            _restartCommand.Subscribe(_ => OnRestartButtonClicked());
            _thirdPopupCommand.Subscribe(_ => OnThirdPopupButtonClicked());
        }

        public async UniTask Enter(object param)
        {
            _ticTacScreenModel.InitializeGame();
            _newModuleScreenView.gameObject.SetActive(false);
            SubscribeToUIUpdates();
            _ticTacScreenView.SetupEventListeners(_mainMenuCommand, _cellCommand, _restartCommand, 
                _thirdPopupCommand);
            _ticTacScreenView.ClearBoard();
            await _newModuleScreenView.Show();
        }

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit() => await _ticTacScreenView.Hide();

        public void Dispose()
        {
            _newModuleScreenView.Dispose();
            _newModuleScreenModel.Dispose();
        }

        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunPresenter(screen);
        }

        private void OnMainMenuButtonClicked() => RunNewScreen(ScreenPresenterMap.MainMenu);

        private void OnCellClicked(int x, int y)
        {
            _ticTacScreenModel.MakeMove(x, y);
            _ticTacScreenView.UpdateBoard(_ticTacScreenModel.board);
            char winner = _ticTacScreenModel.CheckWinner();
            if (winner != '\0')
            {
                _ticTacScreenView.ShowWinner(winner);
                _ticTacScreenView.BlockBoard();
                _ticTacScreenView.AnimateRestartButton();
            }
            else if (_ticTacScreenModel.IsBoardFull())
            {
                _ticTacScreenView.ShowDraw();
                _ticTacScreenView.BlockBoard();
                _ticTacScreenView.AnimateRestartButton();
            }
        }

        private void OnRestartButtonClicked()
        {
            _ticTacScreenModel.InitializeGame();
            _ticTacScreenView.ClearBoard();
            _ticTacScreenView.UnblockBoard(); 
            _ticTacScreenView.StopAnimateRestartButton();
        }

        private void OnThirdPopupButtonClicked() => _popupHub.OpenThirdPopup();
    }
}
