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
        [SerializeField] private CellUIView[] cellViews;
        [SerializeField] private TMP_Text winnerText;
        private const int BoardSize = 3;

        private void Awake()
        {
            if (cellViews.Length != BoardSize * BoardSize)
            {
                Debug.LogError("The number of cell views should be equal to " + (BoardSize * BoardSize));
            }
        }

        public void SetupEventListeners(UnityAction onMainMenuButtonClicked, UnityAction<int, int> onCellClicked)
        {
            mainMenuButton.onClick.AddListener(onMainMenuButtonClicked);
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
            foreach (var cellView in cellViews)
            {
                cellView.ClearEventListeners();
            }
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

        public void ShowWinner(char winner) => winnerText.text = $"Winner: {winner}";
    }
}