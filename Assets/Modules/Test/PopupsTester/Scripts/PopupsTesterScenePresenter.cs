using System;
using System.Collections.Generic;
using CodeBase.Core.Modules;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Core.Systems.PopupHub;
using CodeBase.Services;
using CodeBase.Services.EventMediator;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterScenePresenter : IScreenPresenter, IStartable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly PopupsTesterSceneModuleModel _popupsTesterSceneModuleModel;
        private readonly PopupsTesterSceneView _popupsTesterSceneView;
        private readonly Func<Action, TestButtonView> _buttonFactory;
        private readonly List<TestButtonView> _buttons = new();
        private readonly EventMediator _eventMediator;

        private readonly Dictionary<TestButtonView, ReactiveCommand<Unit>> _buttonCommandMap = new();

        public PopupsTesterScenePresenter(Func<Action, TestButtonView> buttonFactory, EventMediator eventMediator,
            PopupsTesterSceneView popupsTesterSceneView, PopupsTesterSceneModuleModel popupsTesterSceneModuleModel)
        {
            _popupsTesterSceneView = popupsTesterSceneView;
            _popupsTesterSceneModuleModel = popupsTesterSceneModuleModel;
            _buttonFactory = buttonFactory;
            _eventMediator = eventMediator;
        }

        public void Start() => Enter(null).Forget();
        
        public async UniTask Enter(object param)
        {
            var popupActions = _popupsTesterSceneModuleModel.GetPopupHubActions();
            foreach (var action in popupActions)
                CreateButton(action);

            Initialize();

            _eventMediator.OnPopupOpenedAsObservable()
                .Subscribe(OnPopupOpened)
                .AddTo(_disposables);

            _popupsTesterSceneView.SetupListeners(_buttonCommandMap);
            await ShowView();
        }

        public async UniTask Execute()
        {
            await UniTask.WaitUntil(() => !Application.isPlaying);
            await Exit();
        }

        public async UniTask Exit()
        {
            await HideScreenView();
            Dispose();
        }

        private static void OnPopupOpened(PopupOpenedEvent popupEvent) => 
            Debug.Log($"Open PopupHub: {popupEvent.PopupName}");

        private void Initialize() => _popupsTesterSceneView.GetPopupsButtons(_buttons);

        private async UniTask ShowView() => await _popupsTesterSceneView.Show();

        private async UniTask HideScreenView() => await _popupsTesterSceneView.Hide();

        private void CreateButton(Action action)
        {
            var button = _buttonFactory(action);
            _buttons.Add(button);

            var reactiveCommand = new ReactiveCommand<Unit>();
            _buttonCommandMap[button] = reactiveCommand;

            reactiveCommand.Subscribe(_ => button.Show().Forget()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _popupsTesterSceneView.Dispose();
            _popupsTesterSceneModuleModel.Dispose();
        }
    }
}