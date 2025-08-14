using System;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub;
using Cysharp.Threading.Tasks;
using MediatR;
using R3;
using UnityEngine;
using Unit = R3.Unit;

namespace Modules.Base.ServerSampleModule.Scripts
{
    /// <summary>
    /// Request handler for ServerSample module operations
    /// </summary>
    public class ServerSampleRequest : IRequest<string> { }

    /// <summary>
    /// Handler for ServerSample module requests
    /// </summary>
    public class ServerSampleHandler : IRequestHandler<ServerSampleRequest, string>
    {
        public Task<string> Handle(ServerSampleRequest request, CancellationToken cancellationToken) => 
            Task.FromResult("ServerSample Handler Invoked!");
    }
    
    /// <summary>
    /// Presenter for ServerSample module that handles business logic and coordinates between Model and View
    /// 
    /// IMPORTANT: This is a serverSample file for ModuleCreator system.
    /// When creating a new module, this file will be copied and modified.
    /// 
    /// Key points for customization:
    /// 1. Change class name from ServerSamplePresenter to YourModuleNamePresenter
    /// 2. Update namespace Modules.Base.ServerSampleModule.Scripts match your module location
    /// 3. Add your specific business logic and commands
    /// 4. Customize module navigation logic
    /// 5. Implement your specific UI event handling
    /// 6. Add any additional services or systems your module needs
    /// 
    /// NOTE: Navigation to MainMenuModule is already implemented via exit button
    /// </summary>
    public class ServerSamplePresenter : IDisposable
    {
        private readonly ServerSampleModuleModel _serverSampleModuleModel;
        private readonly ServerSampleView _serverSampleView;
        private readonly AudioSystem _audioSystem;
        private readonly IPopupHub _popupHub;
        
        private readonly CompositeDisposable _disposables = new();
        
        private ReactiveCommand<ModulesMap> _openNewModuleCommand;
        private readonly ReactiveCommand<Unit> _openMainMenuCommand = new();
        private readonly ReactiveCommand<Unit> _settingsPopupCommand = new();
        private readonly ReactiveCommand<bool> _toggleSoundCommand = new();

        public ServerSamplePresenter(
            ServerSampleModuleModel serverSampleModuleModel,
            ServerSampleView serverSampleView,
            AudioSystem audioSystem,
            IPopupHub popupHub)
        {
            _serverSampleModuleModel = serverSampleModuleModel ?? throw new ArgumentNullException(nameof(serverSampleModuleModel));
            _serverSampleView = serverSampleView ?? throw new ArgumentNullException(nameof(serverSampleView));
            _audioSystem = audioSystem ?? throw new ArgumentNullException(nameof(audioSystem));
            _popupHub = popupHub ?? throw new ArgumentNullException(nameof(popupHub));
        }

        public async UniTask Enter(ReactiveCommand<ModulesMap> runModuleCommand)
        {
            _openNewModuleCommand = runModuleCommand ?? throw new ArgumentNullException(nameof(runModuleCommand));
            
            _serverSampleView.HideInstantly();

            var commands = new ServerSampleCommands(
                _openMainMenuCommand,
                _settingsPopupCommand,
                _toggleSoundCommand
            );

            _serverSampleView.SetupEventListeners(commands);
            SubscribeToUIUpdates();

            _serverSampleView.InitializeSoundToggle(isMusicOn: _audioSystem.MusicVolume != 0);
            await _serverSampleView.Show();
            
            _audioSystem.PlayMainMenuMelody();
        }

        public async UniTask Exit()
        {
            await _serverSampleView.Hide();
        }
        
        public void HideInstantly() => _serverSampleView.HideInstantly();

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void SubscribeToUIUpdates()
        {
            _openMainMenuCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_serverSampleModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnMainMenuButtonClicked())
                .AddTo(_disposables);

            _settingsPopupCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_serverSampleModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnSettingsPopupButtonClicked())
                .AddTo(_disposables);

            _toggleSoundCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_serverSampleModuleModel.CommandThrottleDelay))
                .Subscribe(OnSoundToggled)
                .AddTo(_disposables);
        }

        private void OnMainMenuButtonClicked()
        {
            _openNewModuleCommand.Execute(ModulesMap.MainMenu);
        }

        private void OnSettingsPopupButtonClicked()
        {
            _popupHub.OpenSettingsPopup();
        }

        private void OnSoundToggled(bool isOn)
        {
            _audioSystem.SetMusicVolume(isOn ? 1f : 0f);
        }
    }
}
