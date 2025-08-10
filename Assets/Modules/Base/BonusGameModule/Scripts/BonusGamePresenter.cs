using System;
using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub;
using Cysharp.Threading.Tasks;
using MediatR;
using R3;
using Unit = R3.Unit;

namespace Modules.Base.BonusGameModule.Scripts
{
    /// <summary>
    /// Request handler for BonusGame module operations
    /// </summary>
    public class BonusGameRequest : IRequest<string> { }

    /// <summary>
    /// Handler for BonusGame module requests
    /// </summary>
    public class BonusGameHandler : IRequestHandler<BonusGameRequest, string>
    {
        public Task<string> Handle(BonusGameRequest request, CancellationToken cancellationToken) => 
            Task.FromResult("BonusGame Handler Invoked!");
    }
    
    /// <summary>
    /// Presenter for BonusGame module that handles business logic and coordinates between Model and View
    /// 
    /// IMPORTANT: This is a bonusGame file for ModuleCreator system.
    /// When creating a new module, this file will be copied and modified.
    /// 
    /// Key points for customization:
    /// 1. Change class name from BonusGamePresenter to YourModuleNamePresenter
    /// 2. Update namespace Modules.Base.BonusGameModule.Scripts match your module location
    /// 3. Add your specific business logic and commands
    /// 4. Customize module navigation logic
    /// 5. Implement your specific UI event handling
    /// 6. Add any additional services or systems your module needs
    /// </summary>
    public class BonusGamePresenter : IDisposable
    {
        private readonly BonusGameModuleModel _bonusGameModuleModel;
        private readonly BonusGameView _bonusGameView;
        private readonly AudioSystem _audioSystem;
        private readonly IPopupHub _popupHub;
        
        private readonly CompositeDisposable _disposables = new();
        
        private ReactiveCommand<ModulesMap> _openNewModuleCommand;
        private readonly ReactiveCommand<Unit> _openMainMenuCommand = new();
        private readonly ReactiveCommand<Unit> _settingsPopupCommand = new();
        private readonly ReactiveCommand<bool> _toggleSoundCommand = new();

        public BonusGamePresenter(
            BonusGameModuleModel bonusGameModuleModel,
            BonusGameView bonusGameView,
            AudioSystem audioSystem,
            IPopupHub popupHub)
        {
            _bonusGameModuleModel = bonusGameModuleModel ?? throw new ArgumentNullException(nameof(bonusGameModuleModel));
            _bonusGameView = bonusGameView ?? throw new ArgumentNullException(nameof(bonusGameView));
            _audioSystem = audioSystem ?? throw new ArgumentNullException(nameof(audioSystem));
            _popupHub = popupHub ?? throw new ArgumentNullException(nameof(popupHub));
        }

        public async UniTask Enter(ReactiveCommand<ModulesMap> runModuleCommand)
        {
            _openNewModuleCommand = runModuleCommand ?? throw new ArgumentNullException(nameof(runModuleCommand));
            
            _bonusGameView.HideInstantly();

            var commands = new BonusGameCommands(
                _openMainMenuCommand,
                _settingsPopupCommand,
                _toggleSoundCommand
            );

            _bonusGameView.SetupEventListeners(commands);

            _bonusGameView.InitializeSoundToggle(isMusicOn: _audioSystem.MusicVolume != 0);
            await _bonusGameView.Show();
            
            _audioSystem.PlayMainMenuMelody();
        }

        public async UniTask Exit()
        {
            await _bonusGameView.Hide();
        }
        
        public void HideInstantly() => _bonusGameView.HideInstantly();

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void SubscribeToUIUpdates()
        {
            _openMainMenuCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_bonusGameModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnMainMenuButtonClicked())
                .AddTo(_disposables);

            _settingsPopupCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_bonusGameModuleModel.CommandThrottleDelay))
                .Subscribe(_ => OnSettingsPopupButtonClicked())
                .AddTo(_disposables);

            _toggleSoundCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_bonusGameModuleModel.CommandThrottleDelay))
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
