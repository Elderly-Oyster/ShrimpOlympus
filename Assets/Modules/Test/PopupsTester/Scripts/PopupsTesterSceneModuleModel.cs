using System;
using CodeBase.Core.Modules;
using CodeBase.Core.Modules.MVP;
using CodeBase.Core.Systems.PopupHub;
using VContainer;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneModuleModel : IModuleModel
    {
        private readonly Func<Action, TestButtonView> _buttonFactory;
        private readonly Action[] _popupActions;

        [Inject] public PopupsTesterSceneModuleModel(IPopupHub popupHub)
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