using Core.Popup.Base;
using System;
using Core.MVP;
using VContainer;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneModel : IScreenModel
    {
        private readonly Func<Action, TestButtonView> _buttonFactory;
        private readonly Action[] _popupActions;

        [Inject] public PopupsTesterSceneModel(PopupHub popupHub)
        {
            var popupHub1 = popupHub;

            _popupActions = new Action[]
            {
                popupHub1.OpenFirstPopup,
                popupHub1.OpenSecondPopup,
                popupHub1.OpenThirdPopup
            };
        }

        public Action[] GetPopupHubActions() => _popupActions;

        public void Dispose() { }
    }
}