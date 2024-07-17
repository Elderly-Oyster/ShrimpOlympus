using UnityEngine;
using UnityEngine.UI;

namespace MVP.MVP_Root_Model.Scripts.General.PromotionAdditionalScene
{
    public class PromotionADView : MonoBehaviour
    {
        [SerializeField] private Button openPopupButton;
        [SerializeField] private PromotionPopup popup;

        private void Awake()
        {
            openPopupButton.onClick.AddListener(OpenPopup);
        }

        private void OpenPopup() => popup.OpenPopup();
    }
}