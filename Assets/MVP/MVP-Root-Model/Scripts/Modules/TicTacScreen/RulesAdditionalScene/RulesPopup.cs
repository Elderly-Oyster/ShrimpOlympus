using UnityEngine;
using UnityEngine.UI;

namespace MVP.MVP_Root_Model.Scripts.Modules.TicTacScreen.AdditionalScene
{
    public class RulesPopup : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        private void Awake()
        {
            OpenPopup();
            closeButton.onClick.AddListener(ClosePopup);
        }

        private void OpenPopup()
        {
            gameObject.SetActive(true);
        }
        
        private void ClosePopup()
        {
            gameObject.SetActive(false);
        }
    }
}