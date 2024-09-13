using System;
using Core.MVP;
using Core.Views.Buttons;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Base.TicTacScreen.Scripts
{
    public class TicTacScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button thirdPopupButton;
        [SerializeField] private PulsatingButton restartButton;
        [SerializeField] private CellUIView[] cellViews;
        [SerializeField] private TMP_Text winnerText;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private const int BoardSize = 3;

        protected override void Awake()
        {
            if (cellViews.Length != BoardSize * BoardSize)
                Debug.LogError("The number of cell views should be equal to " + (BoardSize * BoardSize));
            ClearBoard();
            
            base.Awake();
            HideInstantly();
        }

        public void SetupEventListeners(ReactiveCommand onMainMenuButtonClicked, ReactiveCommand<int[]> onCellClicked, 
            ReactiveCommand onRestartButtonClicked, ReactiveCommand onThirdPopupButtonClicked)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => onMainMenuButtonClicked.Execute())
                .AddTo(_disposables);

            thirdPopupButton.OnClickAsObservable()
                .Subscribe(_ => onThirdPopupButtonClicked.Execute())
                .AddTo(_disposables);

            restartButton.pulsatingButton.OnClickAsObservable()
                .Subscribe(_ => onRestartButtonClicked.Execute())
                .AddTo(_disposables);

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    int index = i * BoardSize + j;
                    cellViews[index].Initialize(i, j, onCellClicked);
                }
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
        
        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners() => _disposables.Clear();
    }
}
