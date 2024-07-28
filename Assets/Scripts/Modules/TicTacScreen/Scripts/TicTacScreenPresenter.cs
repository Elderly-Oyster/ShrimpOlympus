using Core;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Modules.TicTacScreen.Scripts
{
    public class TicTacScreenPresenter : IPresenter
    {
        [Inject] private readonly TicTacScreenView _ticTacScreenView;
        private TicTacScreenModel _ticTacScreenModel;
        private readonly UniTaskCompletionSource<bool> _completionSource = new();

        public void Initialize(TicTacScreenModel ticTacScreenModel)
        {
            _ticTacScreenModel = ticTacScreenModel;
            _ticTacScreenView.gameObject.SetActive(false);
            _ticTacScreenView.SetupEventListeners(OnMainMenuButtonClicked, OnCellClicked, OnRestartButtonClicked, 
                OnThirdPopupButtonClicked);
            _ticTacScreenView.ClearBoard();
        }

        public async UniTask ShowView() => await _ticTacScreenView.Show();

        private void OnMainMenuButtonClicked()
        {
            _ticTacScreenModel.RunMainMenuModel();
            _completionSource.TrySetResult(true);
        }

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
        private void OnThirdPopupButtonClicked() => _ticTacScreenModel.OpenThirdPopup();

        public async UniTask WaitForTransitionButtonPress() => await _completionSource.Task;

        public void RemoveEventListeners() => _ticTacScreenView.RemoveEventListeners();

        public async UniTask HideScreenView() => await _ticTacScreenView.Hide();
    }
}
