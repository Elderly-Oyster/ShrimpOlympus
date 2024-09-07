using Core;
using Core.MVVM;
using Core.Popup.Base;
using Cysharp.Threading.Tasks;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly MainMenuScreenModel _mainMenuScreenModel;
        private readonly MainMenuScreenView _mainMenuScreenView;
        private readonly PopupHub _popupHub;
        private readonly UniTaskCompletionSource<bool> _completionSource;

        public MainMenuScreenPresenter(IScreenStateMachine screenStateMachine,
            MainMenuScreenModel mainMenuScreenModel, MainMenuScreenView mainMenuScreenView, PopupHub popupHub)
        {
            _screenStateMachine = screenStateMachine;
            _mainMenuScreenModel = mainMenuScreenModel;
            _mainMenuScreenView = mainMenuScreenView;
            _popupHub = popupHub;
            _completionSource = new UniTaskCompletionSource<bool>();
        }

        public async UniTask Enter(object param)
        {
            _mainMenuScreenView.gameObject.SetActive(false);
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