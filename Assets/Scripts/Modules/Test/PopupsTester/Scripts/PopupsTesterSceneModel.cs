using Core;
using Core.Popup.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneModel : IScreenModel, IStartable
    {
        private readonly System.Func<UnityAction, TestButtonView> _buttonFactory;
        private readonly PopupsTesterScenePresenter _popupsTesterScenePresenter;
        private readonly UnityAction[] _popupActions;
        private readonly PopupHub _popupHub;
        private readonly List<TestButtonView> _buttons;

        [Inject] 
        public PopupsTesterSceneModel(
            PopupsTesterScenePresenter popupsTesterScenePresenter,
            PopupHub popupHub,
            System.Func<UnityAction, TestButtonView> buttonFactory)
        {
            _popupsTesterScenePresenter = popupsTesterScenePresenter;
            _popupHub = popupHub;
            _buttonFactory = buttonFactory;
            _buttons = new List<TestButtonView>();

            _popupActions = new UnityAction[]
            {
                _popupHub.OpenFirstPopup,
                _popupHub.OpenSecondPopup,
                _popupHub.OpenThirdPopup
            };
        }

        public void Start() => Run(null).Forget();

        public async UniTask Run(object param)
        {
            _popupsTesterScenePresenter.Initialize(this);

            foreach (var action in _popupActions)
                CreateButton(action);
            
            await _popupsTesterScenePresenter.ShowView();
        }
        
        private void CreateButton(UnityAction action)
        {
            var button = _buttonFactory(action);
            _buttons.Add(button);
        }

        public List<TestButtonView> GetButtons() => _buttons;

        public async UniTask Stop() => await _popupsTesterScenePresenter.HideScreenView();

        public void Dispose() => _popupsTesterScenePresenter.RemoveEventListeners();
    }
}
