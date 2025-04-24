using CodeBase.Core.Infrastructure;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.EventSystems;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IModuleController
    {
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly MainMenuScreenModel _mainMenuScreenModel;
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly MainMenuView _mainMenuView;
        private readonly IPopupHub _popupHub;
        private readonly AudioSystem _audioSystem;
        private readonly InputSystemService _eventSystemService;

        private readonly ReactiveCommand<Unit> _secondPopupCommand = new();
        private readonly ReactiveCommand<Unit> _settingsPopupCommand = new();
        private readonly ReactiveCommand<Unit> _converterCommand = new();
        private readonly ReactiveCommand<Unit> _ticTacCommand = new();
        private readonly ReactiveCommand<Unit> _tycoonCommand = new();
        private readonly ReactiveCommand<bool> _toggleSoundCommand = new();
        
        public MainMenuScreenPresenter(IScreenStateMachine screenStateMachine, IPopupHub popupHub,
            MainMenuScreenModel mainMenuScreenModel, MainMenuView mainMenuView,
            AudioSystem audioSystem)
        {
            _completionSource = new UniTaskCompletionSource<bool>();

            _mainMenuScreenModel = mainMenuScreenModel;
            _screenStateMachine = screenStateMachine;
            _mainMenuView = mainMenuView;
            _audioSystem = audioSystem;
            _popupHub = popupHub;

            SubscribeToUIUpdates();
        }

        private void SubscribeToUIUpdates()
        {
            _secondPopupCommand.Subscribe(_ => OnSecondPopupButtonClicked());
            _settingsPopupCommand.Subscribe(_ => OnSettingsPopupButtonClicked());
            _converterCommand.Subscribe(_ => OnConverterButtonClicked());
            _ticTacCommand.Subscribe(_ => OnTicTacButtonClicked());
            _tycoonCommand.Subscribe(_ => OnTycoonButtonClicked());
            _toggleSoundCommand.Subscribe(OnSoundToggled);
        }

        public async UniTask Enter(object param)
        {
            _mainMenuView.Initialize(isMusicOn: _audioSystem.MusicVolume != 0);
            _mainMenuView.HideInstantly();

            _mainMenuView.SetupEventListeners(
                _converterCommand,
                _ticTacCommand,
                _tycoonCommand,
                _settingsPopupCommand,
                _secondPopupCommand,
                _toggleSoundCommand
            );

            await _mainMenuView.Show();
            _audioSystem.PlayMainMenuMelody();
        }

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit()
        {
            await _mainMenuView.Hide();
            _audioSystem.StopMusic();
        }

        public void Dispose()
        {
            _mainMenuView.Dispose();
            _mainMenuScreenModel.Dispose();
        }

        private void OnConverterButtonClicked() => RunNewScreen(ScreenPresenterMap.Converter);
        private void OnTicTacButtonClicked() => RunNewScreen(ScreenPresenterMap.TicTac);
        private void OnTycoonButtonClicked() => RunNewScreen(ScreenPresenterMap.DeliveryTycoon);
        private void OnSettingsPopupButtonClicked() => _popupHub.OpenSettingsPopup();
        private void OnSecondPopupButtonClicked() => _popupHub.OpenSecondPopup();
        private void OnSoundToggled(bool isOn) => _audioSystem.SetMusicVolume(isOn ? 1 : 0);

        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }
    }
}