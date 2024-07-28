using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Scripts.Core.Popup.Scripts.PopupTest
{
    public class PopupTestScreenView : MonoBehaviour
    {
        private PopupHub _popupHub;
        [SerializeField] private TestButtonView buttonPrefab;
        [SerializeField] private Transform buttonsParent;
        
        
        [Inject] public void Run(PopupHub popupHub)
        {
            _popupHub = popupHub;
            
            CreateButton("FirstPopup", _popupHub.OpenFirstPopup);
            CreateButton("SecondPopup", _popupHub.OpenSecondPopup);
            CreateButton("ThirdPopup", _popupHub.OpenThirdPopup);
        }

        private void CreateButton(string popupName, UnityAction action)
        {
            var testButton = Instantiate(buttonPrefab, buttonsParent).GetComponent<TestButtonView>();
            testButton.gameObject.SetActive(true);
            testButton.label.text = popupName;
            testButton.button.onClick.AddListener(action);
        }
    }
}