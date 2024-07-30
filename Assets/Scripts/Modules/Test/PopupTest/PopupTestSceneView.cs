using Core.Popup.Scripts;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Modules.Test.PopupTest
{
    public class PopupTestSceneView : MonoBehaviour
    {
        private PopupHub _popupHub;
        [SerializeField] private TestButtonView buttonPrefab;
        [SerializeField] private Transform buttonsParent;

        private UnityAction[] _popupActions;

        [Inject] public void Run(PopupHub popupHub)
        {
            _popupHub = popupHub;

            _popupActions = new UnityAction[]
            {
                _popupHub.OpenFirstPopup,
                _popupHub.OpenSecondPopup,
                _popupHub.OpenThirdPopup
            };

            foreach (var action in _popupActions)
                CreateButton(GetMethodName(action), action);
        }

        private void CreateButton(string popupName, UnityAction action)
        {
            var testButton = Instantiate(buttonPrefab, buttonsParent).GetComponent<TestButtonView>();
            testButton.gameObject.SetActive(true);
            testButton.label.text = popupName;
            testButton.button.onClick.AddListener(action);
        }

        private string GetMethodName(UnityAction action) => action.Method.Name;
    }
}