using Core.Popup.Base;
using UnityEngine.Events;
using VContainer;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneModel
    {
        private readonly System.Func<UnityAction, TestButtonView> _buttonFactory;
        private readonly UnityAction[] _popupActions;
        private readonly PopupHub _popupHub;

        [Inject] 
        public PopupsTesterSceneModel(
            PopupHub popupHub, System.Func<UnityAction, TestButtonView> buttonFactory)
        {
            _popupHub = popupHub;

            _popupActions = new UnityAction[]
            {
                _popupHub.OpenFirstPopup,
                _popupHub.OpenSecondPopup,
                _popupHub.OpenThirdPopup
            };
        }

        public UnityAction[] GetPopupHubActions() => _popupActions;
    }
}
