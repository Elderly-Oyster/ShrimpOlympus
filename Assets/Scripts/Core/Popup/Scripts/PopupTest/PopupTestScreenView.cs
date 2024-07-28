using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Scripts.Core.Popup.Scripts.PopupTest
{
    public class PopupTestScreenView : MonoBehaviour
    {
        private PopupHub _popupHub;
        [SerializeField] private Button firstPopupButton;
        [SerializeField] private Button secondPopupButton;
        [SerializeField] private Button thirdPopupButton;
        
        [Inject] public void Run(PopupHub popupHub)
        {
            _popupHub = popupHub;
            Debug.Log(_popupHub);

            firstPopupButton.onClick.AddListener(OpenFirstPopup);
            secondPopupButton.onClick.AddListener(OpenSecondPopup);
            thirdPopupButton.onClick.AddListener(OpenThirdPopup);
        }

        private void OpenThirdPopup() => _popupHub.OpenThirdPopup();
        private void OpenSecondPopup() => _popupHub.OpenSecondPopup();

        private void OpenFirstPopup()
        {
            Debug.Log(_popupHub);
            _popupHub.OpenFirstPopup();
        } 
    }
}