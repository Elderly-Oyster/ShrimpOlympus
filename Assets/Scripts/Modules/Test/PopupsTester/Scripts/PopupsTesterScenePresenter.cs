using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Core;
using Core.EventMediatorSystem;
using UnityEngine;
using VContainer.Unity;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterScenePresenter : ISmartPresenter, IStartable
    {
        private readonly PopupsTesterSceneView _popupsTesterSceneView;
        private readonly PopupsTesterSceneModel _popupsTesterSceneModel;
        private readonly Func<Action, TestButtonView> _buttonFactory;

        public PopupsTesterScenePresenter(Func<Action, TestButtonView> buttonFactory, EventMediator eventMediator,
            PopupsTesterSceneView popupsTesterSceneView, PopupsTesterSceneModel popupsTesterSceneModel)
        {
            _popupsTesterSceneView = popupsTesterSceneView;
            _popupsTesterSceneModel = popupsTesterSceneModel;
            _buttonFactory = buttonFactory;

            eventMediator.Subscribe<PopupOpenedEvent>(OnPopupOpened);
        }
        
        private readonly List<TestButtonView> _buttons = new();
        
        public void Start() => Run(null).Forget();
        
        public async UniTask Run(object param)
        {
            var popupActions = _popupsTesterSceneModel.GetPopupHubActions();
            foreach (var action in popupActions)
                CreateButton(action);
            
            Initialize();

            await ShowView();
        }
        
        private void OnPopupOpened(PopupOpenedEvent popupEvent) => 
            Debug.Log($"Open Popup: {popupEvent.PopupName}");

        private void Initialize() => _popupsTesterSceneView.GetPopupsButtons(_buttons);

        private async UniTask ShowView() => await _popupsTesterSceneView.Show();
        
        private async UniTask HideScreenView() => await _popupsTesterSceneView.Hide();

        private void CreateButton(Action action)
        {
            var button = _buttonFactory(action);
            _buttons.Add(button);
        }
        
        public async UniTask Stop() => await HideScreenView();
    }
}