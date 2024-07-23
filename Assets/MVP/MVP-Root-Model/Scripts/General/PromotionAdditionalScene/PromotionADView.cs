using MVP.MVP_Root_Model.Scripts.Core.Popup.Scripts;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.General.PromotionAdditionalScene
{
    public class PromotionADView : MonoBehaviour
    {
        [Inject] private PopupHub _popupHub;
        [SerializeField] private Button openPopupButton;

        private void Awake()
        {
            openPopupButton.onClick.AddListener(OpenPromotionPopup);
        }

        private void OpenPromotionPopup() => _popupHub.OpenPromotionPopup();
    }
}