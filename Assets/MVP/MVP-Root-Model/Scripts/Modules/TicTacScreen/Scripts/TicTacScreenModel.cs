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

        public char[,] Board { get; private set; }
        public char CurrentPlayer { get; private set; }

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

        private void InitializeGame()
        {
            Board = new char[BoardSize, BoardSize];
            CurrentPlayer = PlayerX;
        }

        public void MakeMove(int x, int y)
        {
            if (Board[x, y] == '\0')
            {
                Board[x, y] = CurrentPlayer;
                CurrentPlayer = CurrentPlayer == PlayerX ? PlayerO : PlayerX;
            }
        }

        public char CheckWinner()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (Board[i, 0] == Board[i, 1] && Board[i, 1] == Board[i, 2] && Board[i, 0] != '\0')
                    return Board[i, 0];
                if (Board[0, i] == Board[1, i] && Board[1, i] == Board[2, i] && Board[0, i] != '\0')
                    return Board[0, i];
            }

            if (Board[0, 0] == Board[1, 1] && Board[1, 1] == Board[2, 2] && Board[0, 0] != '\0')
                return Board[0, 0];
            if (Board[0, 2] == Board[1, 1] && Board[1, 1] == Board[2, 0] && Board[0, 2] != '\0')
                return Board[0, 2];

            return '\0';
        }

        public void RunMainMenuModel() => _rootController.RunModel(ScreenModelMap.MainMenu);
        
        public async UniTask Stop() => await _ticTacScreenPresenter.HideScreenView();
        public void Dispose() => _ticTacScreenPresenter.RemoveEventListeners();
    }
}
