using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub;
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
    
    public class MainMenuPresenter
    {
        [Inject] private IMediator _mediator;
        private readonly MainMenuModuleModel _mainMenuModuleModel;
        private readonly InputSystemService _eventSystemService;
        private readonly MainMenuView _mainMenuView;
        private readonly IPopupHub _popupHub;
        private readonly AudioSystem _audioSystem;
        
        private readonly ReactiveCommand<Unit> _openConverterCommand = new();
        private readonly ReactiveCommand<Unit> _openTicTacCommand = new();
        private readonly ReactiveCommand<Unit> _openTycoonCommand = new();
        private readonly ReactiveCommand<Unit> _settingsPopupCommand = new();
        private readonly ReactiveCommand<Unit> _secondPopupCommand = new();
        private readonly ReactiveCommand<bool> _toggleSoundCommand = new();
        
        private ReactiveCommand<ModulesMap> _openNewModuleCommand = new();
        
        public MainMenuPresenter(IPopupHub popupHub, MainMenuModuleModel mainMenuModuleModel, 
            MainMenuView mainMenuView, AudioSystem audioSystem)
        {
            _mainMenuModuleModel = mainMenuModuleModel;
            _mainMenuView = mainMenuView;
            _audioSystem = audioSystem;
            _popupHub = popupHub;

            SubscribeToUIUpdates();
        }

        private void SubscribeToUIUpdates()
        {
            _openConverterCommand.Subscribe(_ => OnConverterButtonClicked());
            _openTicTacCommand.Subscribe(_ => OnTicTacButtonClicked());
            _openTycoonCommand.Subscribe(_ => OnTycoonButtonClicked());
            _settingsPopupCommand.Subscribe(_ => OnSettingsPopupButtonClicked());
            _secondPopupCommand.Subscribe(_ => OnSecondPopupButtonClicked());
            _toggleSoundCommand.Subscribe(OnSoundToggled);
        }

        public async UniTask Enter(ReactiveCommand<ModulesMap> runModuleCommand)
        {
            _openNewModuleCommand = runModuleCommand;
            
            var result = await _mediator.Send(new MainMenuRequest()); 
            Debug.Log($"MediatR request result: {result}");
            
            _mainMenuView.HideInstantly();

            _mainMenuView.SetupEventListeners(
                _openConverterCommand,
                _openTicTacCommand,
                _openTycoonCommand,
                _settingsPopupCommand,
                _secondPopupCommand,
                _toggleSoundCommand
            );

            _mainMenuView.InitializeSoundToggle(isMusicOn: _audioSystem.MusicVolume != 0);
            await _mainMenuView.Show();
            
            _audioSystem.PlayMainMenuMelody();
        }
        
        public async UniTask Exit()
        {
            await _mainMenuView.Hide();
            _audioSystem.StopMusic();
        }
        
        public void HideInstantly() => _mainMenuView.HideInstantly();

        public void Dispose()
        {
            _mainMenuView.Dispose();
            _mainMenuModuleModel.Dispose();
        }

        private void OnConverterButtonClicked() => _openNewModuleCommand.Execute(ModulesMap.Converter);
        private void OnTicTacButtonClicked() => _openNewModuleCommand.Execute(ModulesMap.TicTac);
        private void OnTycoonButtonClicked() => _openNewModuleCommand.Execute(ModulesMap.DeliveryTycoon);
        private void OnSettingsPopupButtonClicked() => _popupHub.OpenSettingsPopup();
        private void OnSecondPopupButtonClicked() => _popupHub.OpenSecondPopup();
        private void OnSoundToggled(bool isOn) => _audioSystem.SetMusicVolume(isOn ? 1 : 0);
    }
}