using Core;
using Core.MVP;
using Core.Popup.Base;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IScreenPresenter
    {
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly MainMenuScreenModel _mainMenuScreenModel;
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly MainMenuScreenView _mainMenuScreenView;
        private readonly PopupHub _popupHub;

        private readonly ReactiveCommand _secondPopupCommand = new ReactiveCommand();
        private readonly ReactiveCommand _firstPopupCommand = new ReactiveCommand();
        private readonly ReactiveCommand _converterCommand = new ReactiveCommand();
        private readonly ReactiveCommand _ticTacCommand = new ReactiveCommand();

        
        public MainMenuScreenPresenter(IScreenStateMachine screenStateMachine, PopupHub popupHub,
            MainMenuScreenModel mainMenuScreenModel, MainMenuScreenView mainMenuScreenView)
        {
            _completionSource = new UniTaskCompletionSource<bool>();

            _mainMenuScreenModel = mainMenuScreenModel;
            _screenStateMachine = screenStateMachine;
            _mainMenuScreenView = mainMenuScreenView;
            _popupHub = popupHub;

            SubscribeToUIUpdates();
        }

        private void SubscribeToUIUpdates()
        {
            _secondPopupCommand.Subscribe(_ => OnSecondPopupButtonClicked());
            _firstPopupCommand.Subscribe(_ => OnFirstPopupButtonClicked());
            _converterCommand.Subscribe(_ => OnConverterButtonClicked());
            _ticTacCommand.Subscribe(_ => OnTicTacButtonClicked());
        }
        
        public async UniTask Enter(object param)
        {
            _mainMenuScreenView.HideInstantly();

            _mainMenuScreenView.SetupEventListeners(
                _converterCommand,
                _ticTacCommand,
                _firstPopupCommand,
                _secondPopupCommand
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
            _screenStateMachine.RunScreen(screen);
        }
    }
}