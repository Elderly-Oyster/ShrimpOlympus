using UnityEngine;
using UnityEngine.UI;

namespace MVP.MVP_Root_Model.Scripts.Modules.General.PromotionAdditionalScene
{
    public class PromotionPopup : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePopup);
        }

        public void OpenPopup() => gameObject.SetActive(true);
        public void ClosePopup() => gameObject.SetActive(false);
    }
}