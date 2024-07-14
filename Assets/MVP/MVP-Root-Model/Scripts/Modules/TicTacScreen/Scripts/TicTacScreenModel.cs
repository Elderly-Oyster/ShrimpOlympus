using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using System;

namespace MVP.MVP_Root_Model.Scripts.Modules.TicTacScreen.Scripts
{
    public class TicTacScreenModel : IScreenModel
    {
        private readonly IRootController _rootController;
        private readonly TicTacScreenPresenter _ticTacScreenPresenter;
        private const int BoardSize = 3;
        private const char PlayerX = 'X';
        private const char PlayerO = 'O';

        public char[,] board { get; private set; }
        public char currentPlayer { get; private set; }
        public bool isGameOver { get; private set; }

        private static readonly int[][] WinPositions = new int[][]
        {
            new[] {0, 0, 0, 1, 0, 2},
            new[] {1, 0, 1, 1, 1, 2}, 
            new[] {2, 0, 2, 1, 2, 2}, 
            new[] {0, 0, 1, 0, 2, 0}, 
            new[] {0, 1, 1, 1, 2, 1}, 
            new[] {0, 2, 1, 2, 2, 2}, 
            new[] {0, 0, 1, 1, 2, 2},
            new[] {0, 2, 1, 1, 2, 0}  
        };

        public TicTacScreenModel(IRootController rootController, TicTacScreenPresenter ticTacScreenPresenter)
        {
            _rootController = rootController;
            _ticTacScreenPresenter = ticTacScreenPresenter;
        }

        public async UniTask Run(object param)
        {
            InitializeGame();
            _ticTacScreenPresenter.Initialize(this);
            await _ticTacScreenPresenter.ShowView();
            await _ticTacScreenPresenter.WaitForTransitionButtonPress();
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

        public void RunMainMenuModel() => _rootController.RunModel(ScreenModelMap.MainMenu);

        public async UniTask Stop() => await _ticTacScreenPresenter.HideScreenView();
        public void Dispose() => _ticTacScreenPresenter.RemoveEventListeners();
    }
}