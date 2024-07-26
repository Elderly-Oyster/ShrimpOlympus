using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Core.Popup.Scripts
{
    public class PopupTestScreenView : MonoBehaviour
    {
        [Inject] private PopupHub _popupHub;
        [SerializeField] private Button firstPopupButton;
        [SerializeField] private Button secondPopupButton;
        [SerializeField] private Button thirdPopupButton;
        
        private void Awake()
        {
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