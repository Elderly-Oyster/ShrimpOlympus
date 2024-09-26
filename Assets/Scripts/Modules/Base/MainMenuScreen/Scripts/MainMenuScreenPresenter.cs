using Core.Scripts;
using Core.Scripts.Implementation;
using Core.Scripts.MVP;
using Core.Scripts.Popup.Base;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Scripts.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IScreenPresenter
    {
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly MainMenuScreenModel _mainMenuScreenModel;
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly MainMenuScreenView _mainMenuScreenView;
        private readonly PopupHub _popupHub;

        private readonly ReactiveCommand<Unit> _secondPopupCommand = new ReactiveCommand<Unit>();
        private readonly ReactiveCommand<Unit> _firstPopupCommand =  new ReactiveCommand<Unit>();
        private readonly ReactiveCommand<Unit> _converterCommand =  new ReactiveCommand<Unit>();
        private readonly ReactiveCommand<Unit> _ticTacCommand =  new ReactiveCommand<Unit>();

        
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