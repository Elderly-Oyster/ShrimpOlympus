using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainer;
using Core;
using UnityEngine.Events;
using VContainer.Unity;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterScenePresenter : ISmartPresenter, IStartable
    {
        [Inject] private readonly PopupsTesterSceneView _popupsTesterSceneView;
        [Inject] private readonly PopupsTesterSceneModel _popupsTesterSceneModel;
        [Inject] private readonly System.Func<UnityAction, TestButtonView> _buttonFactory;
        
        private readonly List<TestButtonView> _buttons = new();

        private void Initialize() => _popupsTesterSceneView.GetPopupsButtons(_buttons);

        public void Start() => Run(null).Forget();

        private async UniTask ShowView() => await _popupsTesterSceneView.Show();

        private void RemoveEventListeners() => _popupsTesterSceneView.RemoveEventListeners();

        private async UniTask HideScreenView() => await _popupsTesterSceneView.Hide();
        public async UniTask Run(object param)
        {
            var popupActions = _popupsTesterSceneModel.GetPopupHubActions();
            foreach (var action in popupActions)
                CreateButton(action);
            
            Initialize();

            await ShowView();
        }
        private void CreateButton(UnityAction action)
        {
            var button = _buttonFactory(action);
            _buttons.Add(button);
        }
        
        public async UniTask Stop() => await HideScreenView();
        public void Dispose() => RemoveEventListeners();
    }
}