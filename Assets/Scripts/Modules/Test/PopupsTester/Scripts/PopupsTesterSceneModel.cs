using Core.Popup.Base;
using UnityEngine.Events;
using VContainer;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneModel
    {
        private readonly System.Func<UnityAction, TestButtonView> _buttonFactory;
        private readonly UnityAction[] _popupActions;

        [Inject] public PopupsTesterSceneModel(PopupHub popupHub)
        {
            var popupHub1 = popupHub;

            _popupActions = new UnityAction[]
            {
                popupHub1.OpenFirstPopup,
                popupHub1.OpenSecondPopup,
                popupHub1.OpenThirdPopup
            };
        }

        public UnityAction[] GetPopupHubActions() => _popupActions;
    }
}
