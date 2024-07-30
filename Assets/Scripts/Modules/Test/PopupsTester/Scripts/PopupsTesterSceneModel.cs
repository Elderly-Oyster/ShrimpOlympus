using Core;
using Core.Popup.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using VContainer;
using VContainer.Unity;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneModel : IScreenModel, IStartable
    {
        private readonly System.Func<UnityAction, TestButtonView> _buttonFactory;
        private readonly PopupsTesterScenePresenter _popupsTesterScenePresenter;
        private readonly UnityAction[] _popupActions;
        private readonly PopupHub _popupHub;
        
        [Inject] 
        public PopupsTesterSceneModel(
            PopupsTesterScenePresenter popupsTesterScenePresenter,
            PopupHub popupHub,
            System.Func<UnityAction, TestButtonView> buttonFactory)
        {
            _popupsTesterScenePresenter = popupsTesterScenePresenter;
            _popupHub = popupHub;
            _buttonFactory = buttonFactory;

            _popupActions = new UnityAction[]
            {
                _popupHub.OpenFirstPopup,
                _popupHub.OpenSecondPopup,
                _popupHub.OpenThirdPopup
            };
        }

        public void Start() => Run(null);

        public UniTask Run(object param)
        {
            foreach (var action in _popupActions)
                CreateButton(action);
            return default;
        }
        
        private void CreateButton(UnityAction action) => _buttonFactory(action);

        public async UniTask Stop() => await _popupsTesterScenePresenter.HideScreenView();

        public void Dispose() => _popupsTesterScenePresenter.RemoveEventListeners();
    }
}