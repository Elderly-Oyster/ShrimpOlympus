using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Infrastructure.Modules;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub;
using CodeBase.Services;
using CodeBase.Services.Input;
using Cysharp.Threading.Tasks;
using MediatR;
using R3;
using UnityEngine;
using VContainer;
using Unit = R3.Unit;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuRequest : IRequest<string> { }

    public class MainMenuHandler : IRequestHandler<MainMenuRequest, string>
    {
        public Task<string> Handle(MainMenuRequest request, CancellationToken cancellationToken) => 
            Task.FromResult("MainMenu Handler Invoked!");
    }
    
    public class MainMenuModuleController : IModuleController
    {
        [Inject] private IMediator _mediator;
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly MainMenuModuleModel _mainMenuModuleModel;
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
        
        public MainMenuModuleController(IScreenStateMachine screenStateMachine, IPopupHub popupHub,
            MainMenuModuleModel mainMenuModuleModel, MainMenuView mainMenuView,
            AudioSystem audioSystem)
        {
            _completionSource = new UniTaskCompletionSource<bool>();

            _mainMenuModuleModel = mainMenuModuleModel;
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
            var result = await _mediator.Send(new MainMenuRequest()); 
            Debug.Log($"MediatR request result: {result}");
            
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
            _mainMenuModuleModel.Dispose();
        }

        private void OnConverterButtonClicked() => RunNewScreen(ModulesMap.Converter);
        private void OnTicTacButtonClicked() => RunNewScreen(ModulesMap.TicTac);
        private void OnTycoonButtonClicked() => RunNewScreen(ModulesMap.DeliveryTycoon);
        private void OnSettingsPopupButtonClicked() => _popupHub.OpenSettingsPopup();
        private void OnSecondPopupButtonClicked() => _popupHub.OpenSecondPopup();
        private void OnSoundToggled(bool isOn) => _audioSystem.SetMusicVolume(isOn ? 1 : 0);

        private void RunNewScreen(ModulesMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunModule(screen);
        }
    }
}