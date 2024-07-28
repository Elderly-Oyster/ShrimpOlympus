using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.TicTacScreen.Scripts
{
    public class CellUIView : MonoBehaviour
    {
        [SerializeField] private Button cellButton;
        [SerializeField] private TMP_Text cellText;
        private int _x, _y;
        private UnityAction<int, int> _onCellClicked;

        public void Initialize(int x, int y, UnityAction<int, int> onCellClicked)
        {
            _x = x;
            _y = y;
            _onCellClicked = onCellClicked;
            cellButton.onClick.AddListener(OnCellClicked);
        }

        private void OnCellClicked() => _onCellClicked.Invoke(_x, _y);

        public void SetText(char text) => cellText.text = text == '\0' ? "" : text.ToString();

        public void ClearEventListeners()
        {
            cellButton.onClick.RemoveAllListeners();
            _onCellClicked = null;
        }

        public void Block(bool isBlocked) => cellButton.interactable = !isBlocked;

        private void OnDestroy() => ClearEventListeners();
    }
}