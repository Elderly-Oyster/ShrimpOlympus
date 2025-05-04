using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Modules.MVP;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Core.Systems.PopupHub;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Base.TicTacScreen.Scripts
{
    public class TicTacScreenPresenter : IModuleController
    {
        private readonly TicTacView _ticTacView;
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly TicTacModuleModel _newModuleModuleModel;
        private readonly TicTacView _newModuleView;
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly TicTacModuleModel _ticTacModuleModel;
        private readonly IPopupHub _popupHub;
        
        private readonly ReactiveCommand<int[]> _cellCommand = new ReactiveCommand<int[]>();
        private readonly ReactiveCommand<Unit> _mainMenuCommand = new ReactiveCommand<Unit>();
        private readonly ReactiveCommand<Unit> _restartCommand = new ReactiveCommand<Unit>();
        private readonly ReactiveCommand<Unit> _thirdPopupCommand = new ReactiveCommand<Unit>();

        public TicTacScreenPresenter(IScreenStateMachine screenStateMachine,
            TicTacModuleModel newModuleModuleModel, TicTacView newModuleView, 
            TicTacView ticTacView, TicTacModuleModel ticTacModuleModel, IPopupHub popupHub)
        {
            _completionSource = new UniTaskCompletionSource<bool>();

            _screenStateMachine = screenStateMachine;
            _newModuleModuleModel = newModuleModuleModel;
            _newModuleView = newModuleView;
            _ticTacView = ticTacView;
            _ticTacModuleModel = ticTacModuleModel;
            _popupHub = popupHub;
        }

        private void SubscribeToUIUpdates()
        {
            _cellCommand.Subscribe(position => OnCellClicked(position[0], position[1]));
            _mainMenuCommand.Subscribe(_ => OnMainMenuButtonClicked());
            _restartCommand.Subscribe(_ => OnRestartButtonClicked());
            _thirdPopupCommand.Subscribe(_ => OnThirdPopupButtonClicked());
        }

        public async UniTask Enter(object param)
        {
            _ticTacModuleModel.InitializeGame();
            _newModuleView.gameObject.SetActive(false);
            SubscribeToUIUpdates();
            _ticTacView.SetupEventListeners(_mainMenuCommand, _cellCommand, _restartCommand, 
                _thirdPopupCommand);
            _ticTacView.ClearBoard();
            await _newModuleView.Show();
        }

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit() => await _ticTacView.Hide();

        public void Dispose()
        {
            _newModuleView.Dispose();
            _newModuleModuleModel.Dispose();
        }

        private void RunNewScreen(ModulesMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }

        private void OnMainMenuButtonClicked() => RunNewScreen(ModulesMap.MainMenu);

        private void OnCellClicked(int x, int y)
        {
            _ticTacModuleModel.MakeMove(x, y);
            _ticTacView.UpdateBoard(_ticTacModuleModel.Board);
            char winner = _ticTacModuleModel.CheckWinner();
            if (winner != '\0')
            {
                _ticTacView.ShowWinner(winner);
                _ticTacView.BlockBoard();
                _ticTacView.AnimateRestartButton();
            }
            else if (_ticTacModuleModel.IsBoardFull())
            {
                _ticTacView.ShowDraw();
                _ticTacView.BlockBoard();
                _ticTacView.AnimateRestartButton();
            }
        }

        private void OnRestartButtonClicked()
        {
            _ticTacModuleModel.InitializeGame();
            _ticTacView.ClearBoard();
            _ticTacView.UnblockBoard(); 
            _ticTacView.StopAnimateRestartButton();
        }

        private void OnThirdPopupButtonClicked() => _popupHub.OpenThirdPopup();
    }
}
