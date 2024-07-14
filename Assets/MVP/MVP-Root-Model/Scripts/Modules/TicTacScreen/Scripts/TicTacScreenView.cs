using MVP.MVP_Root_Model.Scripts.Core.Buttons;
using MVP.MVP_Root_Model.Scripts.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MVP.MVP_Root_Model.Scripts.Modules.TicTacScreen.Scripts
{
    public class TicTacScreenView : UIView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private PulsatingButton restartButton;
        [SerializeField] private CellUIView[] cellViews;
        [SerializeField] private TMP_Text winnerText;
        private const int BoardSize = 3;

        private void Awake()
        {
            if (cellViews.Length != BoardSize * BoardSize)
                Debug.LogError("The number of cell views should be equal to " + (BoardSize * BoardSize));
            ClearBoard();
        }

        public void SetupEventListeners(UnityAction onMainMenuButtonClicked, UnityAction<int, int> onCellClicked, UnityAction onRestartButtonClicked)
        {
            mainMenuButton.onClick.AddListener(onMainMenuButtonClicked);
            restartButton.pulsatingButton.onClick.AddListener(onRestartButtonClicked);
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    int index = i * BoardSize + j;
                    cellViews[index].Initialize(i, j, onCellClicked);
                }
            }
        }

        public void RemoveEventListeners()
        {
            mainMenuButton.onClick.RemoveAllListeners();
            restartButton.pulsatingButton.onClick.RemoveAllListeners();
            foreach (var cellView in cellViews) 
                cellView.ClearEventListeners();
        }

        public void UpdateBoard(char[,] board)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    int index = i * BoardSize + j;
                    cellViews[index].SetText(board[i, j]);
                }
            }
        }

        public void ClearBoard()
        {
            foreach (var cellView in cellViews)
            {
                cellView.SetText('\0');
                cellView.Block(false);
            }
            winnerText.text = "";
        }

        public void ShowWinner(char winner) => winnerText.text = $"Winner: {winner}";

        public void ShowDraw() => winnerText.text = "Draw!";

        public void BlockBoard()
        {
            foreach (var cellView in cellViews) 
                cellView.Block(true);
        }

        public void UnblockBoard()
        {
            foreach (var cellView in cellViews) 
                cellView.Block(false);
        }

        public void AnimateRestartButton() => restartButton.PlayAnimation();

        public void StopAnimateRestartButton() => restartButton.StopAnimation();
    }
}
