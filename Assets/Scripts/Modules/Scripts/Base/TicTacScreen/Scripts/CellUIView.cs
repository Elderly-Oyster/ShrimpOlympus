using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Scripts.Base.TicTacScreen.Scripts
{
    public class CellUIView : MonoBehaviour
    {
        [SerializeField] private Button cellButton;
        [SerializeField] private TMP_Text cellText;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private ReactiveCommand<int[]> _onCellClicked;
        private int _x, _y;

        
        public void Initialize(int x, int y, ReactiveCommand<int[]> cellCommand)
        {
            _x = x;
            _y = y;
            int[] array = {x, y};
            _onCellClicked = cellCommand;

            cellButton.OnClickAsObservable()
                .Subscribe(_ => _onCellClicked?.Execute(array))
                .AddTo(_disposables);
        }

        public void SetText(char text) => cellText.text = text == '\0' ? "" : text.ToString();

        private void ClearEventListeners()
        {
            _disposables.Clear();
            _onCellClicked = null;
        }

        public void Block(bool isBlocked) => cellButton.interactable = !isBlocked;

        private void OnDestroy() => ClearEventListeners();
    }
}