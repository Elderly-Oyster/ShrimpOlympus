using Core;
using Core.MVVM;
using Core.Popup.Scripts;
using Cysharp.Threading.Tasks;

namespace Modules.Base.TicTacScreen.Scripts
{
    public class TicTacScreenModel : IScreenModel
    {
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly TicTacScreenPresenter _ticTacScreenPresenter;
        private readonly IScreenStateMachine _rootStateMachine;
        private readonly PopupHub _popupHub;
        private const int BoardSize = 3;
        private const char PlayerX = 'X';
        private const char PlayerO = 'O';

        public char[,] board { get; private set; }
        public char currentPlayer { get; private set; }
        public bool isGameOver { get; private set; }

        private static readonly int[][] WinPositions = {
            new[] {0, 0, 0, 1, 0, 2},
            new[] {1, 0, 1, 1, 1, 2}, 
            new[] {2, 0, 2, 1, 2, 2}, 
            new[] {0, 0, 1, 0, 2, 0}, 
            new[] {0, 1, 1, 1, 2, 1}, 
            new[] {0, 2, 1, 2, 2, 2}, 
            new[] {0, 0, 1, 1, 2, 2},
            new[] {0, 2, 1, 1, 2, 0}  
        };

        public TicTacScreenModel(IScreenStateMachine rootStateMachine, TicTacScreenPresenter ticTacScreenPresenter, PopupHub popupHub)
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            _ticTacScreenPresenter = ticTacScreenPresenter;
            _rootStateMachine = rootStateMachine;
            _popupHub = popupHub;
        }

        public async UniTask Run(object param)
        {
            InitializeGame();
            _ticTacScreenPresenter.Initialize(this);
            await _ticTacScreenPresenter.ShowView();
            await _completionSource.Task;
        }

        public void InitializeGame()
        {
            board = new char[BoardSize, BoardSize];
            currentPlayer = PlayerX;
            isGameOver = false;
        }

        public void MakeMove(int x, int y)
        {
            if (board[x, y] == '\0' && !isGameOver)
            {
                board[x, y] = currentPlayer;
                currentPlayer = currentPlayer == PlayerX ? PlayerO : PlayerX;
            }
        }

        public char CheckWinner()
        {
            foreach (var pos in WinPositions)
            {
                if (board[pos[0], pos[1]] == board[pos[2], pos[3]] &&
                    board[pos[2], pos[3]] == board[pos[4], pos[5]] &&
                    board[pos[0], pos[1]] != '\0')
                {
                    isGameOver = true;
                    return board[pos[0], pos[1]];
                }
            }
            return '\0';
        }

        public bool IsBoardFull()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] == '\0')
                        return false;
                }
            }
            isGameOver = true;
            return true;
        }

        public void OpenThirdPopup() => _popupHub.OpenThirdPopup();
        
        public void RunMainMenuModel()
        {
            _completionSource.TrySetResult(true);
            _rootStateMachine.RunViewModel(ScreenModelMap.MainMenu);
        }

        public async UniTask Stop() => await _ticTacScreenPresenter.HideScreenView();
        public void Dispose() => _ticTacScreenPresenter.RemoveEventListeners();
    }
}