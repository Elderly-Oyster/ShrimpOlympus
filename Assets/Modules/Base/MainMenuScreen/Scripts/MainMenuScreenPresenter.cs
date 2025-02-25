using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub;
using CodeBase.Systems.PopupHub;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IScreenPresenter
    {
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly MainMenuScreenModel _mainMenuScreenModel;
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly MainMenuScreenView _mainMenuScreenView;
        private readonly IPopupHub _popupHub;
        private readonly AudioSystem _audioSystem;

        private readonly ReactiveCommand<Unit> _secondPopupCommand = new();
        private readonly ReactiveCommand<Unit> _firstPopupCommand = new();
        private readonly ReactiveCommand<Unit> _converterCommand = new();
        private readonly ReactiveCommand<Unit> _ticTacCommand = new();
        private readonly ReactiveCommand<bool> _toggleSoundCommand = new();
        
        public MainMenuScreenPresenter(IScreenStateMachine screenStateMachine, IPopupHub popupHub,
            MainMenuScreenModel mainMenuScreenModel, MainMenuScreenView mainMenuScreenView,
            AudioSystem audioSystem)
        {
            _completionSource = new UniTaskCompletionSource<bool>();

            _mainMenuScreenModel = mainMenuScreenModel;
            _screenStateMachine = screenStateMachine;
            _mainMenuScreenView = mainMenuScreenView;
            _audioSystem = audioSystem;
            _popupHub = popupHub;

            SubscribeToUIUpdates();
        }

        private void SubscribeToUIUpdates()
        {
            _secondPopupCommand.Subscribe(_ => OnSecondPopupButtonClicked());
            _firstPopupCommand.Subscribe(_ => OnFirstPopupButtonClicked());
            _converterCommand.Subscribe(_ => OnConverterButtonClicked());
            _ticTacCommand.Subscribe(_ => OnTicTacButtonClicked());
            _toggleSoundCommand.Subscribe(OnSoundToggled);
        }

        public async UniTask Enter(object param)
        {
            _mainMenuScreenView.Initialize(_audioSystem.MusicVolume != 0);
            _mainMenuScreenView.HideInstantly();

            _mainMenuScreenView.SetupEventListeners(
                _converterCommand,
                _ticTacCommand,
                _firstPopupCommand,
                _secondPopupCommand,
                _toggleSoundCommand
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
        private void OnSoundToggled(bool isOn) => _audioSystem.SetMusicVolume(isOn ? 1 : 0);

        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }
    }
}