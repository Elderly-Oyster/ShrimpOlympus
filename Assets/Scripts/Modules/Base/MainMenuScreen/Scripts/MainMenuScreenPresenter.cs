using Core;
using Core.EventMediatorSystem;
using Core.MVVM;
using Core.Popup.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IScreenPresenter
    {
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly MainMenuScreenModel _mainMenuScreenModel;
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly MainMenuScreenView _mainMenuScreenView;
        private readonly PopupHub _popupHub;

        public MainMenuScreenPresenter(IScreenStateMachine screenStateMachine, PopupHub popupHub,
            MainMenuScreenModel mainMenuScreenModel, MainMenuScreenView mainMenuScreenView)
        {
            _mainMenuScreenModel = mainMenuScreenModel;
            _screenStateMachine = screenStateMachine;
            _mainMenuScreenView = mainMenuScreenView;
            _popupHub = popupHub;
            _completionSource = new UniTaskCompletionSource<bool>();
        }

        public async UniTask Enter(object param)
        {
            _mainMenuScreenView.HideInstantly();
            _mainMenuScreenView.SetupEventListeners
            (
                OnConverterButtonClicked,
                OnTicTacButtonClicked,
                OnFirstPopupButtonClicked,
                OnSecondPopupButtonClicked
            );
            await _mainMenuScreenView.Show();
        }

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit() => await _mainMenuScreenView.Hide();

        public void Dispose()
        {
            _mainMenuScreenView.Dispose();
            _mainMenuScreenModel.Dispose();
        }

        private void OnConverterButtonClicked() => RunNewScreen(ScreenPresenterMap.Converter);
        private void OnTicTacButtonClicked() => RunNewScreen(ScreenPresenterMap.TicTac);
        private void OnFirstPopupButtonClicked() => _popupHub.OpenFirstPopup();
        private void OnSecondPopupButtonClicked() => _popupHub.OpenSecondPopup();
        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunPresenter(screen);
        }
    }
}