using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Core;
using Core.EventMediatorSystem;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterScenePresenter : ISmartPresenter, IStartable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly PopupsTesterSceneModel _popupsTesterSceneModel;
        private readonly PopupsTesterSceneView _popupsTesterSceneView;
        private readonly Func<Action, TestButtonView> _buttonFactory;
        private readonly List<TestButtonView> _buttons = new();
        private readonly EventMediator _eventMediator;

        private readonly ReactiveCommand _firstPopupCommand = new ReactiveCommand();
        private readonly ReactiveCommand _secondPopupCommand = new ReactiveCommand();
        private readonly ReactiveCommand _thirdPopupCommand = new ReactiveCommand();

        
        public PopupsTesterScenePresenter(Func<Action, TestButtonView> buttonFactory, EventMediator eventMediator,
            PopupsTesterSceneView popupsTesterSceneView, PopupsTesterSceneModel popupsTesterSceneModel)
        {
            _popupsTesterSceneView = popupsTesterSceneView;
            _popupsTesterSceneModel = popupsTesterSceneModel;
            _buttonFactory = buttonFactory;
            _eventMediator = eventMediator;
        }
        public void Start() => Run(null).Forget();

        public async UniTask Run(object param)
        {
            var popupActions = _popupsTesterSceneModel.GetPopupHubActions();
            foreach (var action in popupActions)
                CreateButton(action);
            
            Initialize();

            _eventMediator.OnPopupOpenedAsObservable()
                .Subscribe(OnPopupOpened)
                .AddTo(_disposables);

            await ShowView();
        }
        
        private void OnPopupOpened(PopupOpenedEvent popupEvent) => 
            Debug.Log($"Open Popup: {popupEvent.PopupName}");

        private void Initialize() => _popupsTesterSceneView.GetPopupsButtons(_buttons, this);

        private async UniTask ShowView() => await _popupsTesterSceneView.Show();
        
        private async UniTask HideScreenView() => await _popupsTesterSceneView.Hide();

        private void CreateButton(Action action)
        {
            var button = _buttonFactory(action);
            _buttons.Add(button);
        }

        public async UniTask Stop()
        {
            _disposables.Dispose();  
            await HideScreenView();
        }

        public void RegisterButton(TestButtonView button, int index)
        {
            if (index == 1)
            {
                _firstPopupCommand.Subscribe(_ => button.Show().Forget()).AddTo(_disposables);
            }
            else if (index == 2)
            {
                _secondPopupCommand.Subscribe(_ => button.Show().Forget()).AddTo(_disposables);
            }
            else if (index == 3)
            {
                _thirdPopupCommand.Subscribe(_ => button.Show().Forget()).AddTo(_disposables);
            }
        }

    }
}