using CodeBase.Core.UI.Views;
using CodeBase.Services.Input;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Unit = R3.Unit;

namespace Modules.Base.BonusGameModule.Scripts
{
    /// <summary>
    /// Commands structure for BonusGame module UI interactions
    /// </summary>
    public readonly struct BonusGameCommands
    {
        public readonly ReactiveCommand<Unit> OpenMainMenuCommand;
        public readonly ReactiveCommand<Unit> SettingsPopupCommand;
        public readonly ReactiveCommand<bool> SoundToggleCommand;

        public BonusGameCommands(
            ReactiveCommand<Unit> openMainMenuCommand,
            ReactiveCommand<Unit> settingsPopupCommand,
            ReactiveCommand<bool> soundToggleCommand)
        {
            OpenMainMenuCommand = openMainMenuCommand;
            SettingsPopupCommand = settingsPopupCommand;
            SoundToggleCommand = soundToggleCommand;
        }
    }
    
    /// <summary>
    /// View for BonusGame module that handles UI interactions and visual representation
    /// 
    /// IMPORTANT: This is a bonusGame file for ModuleCreator system.
    /// When creating a new module, this file will be copied and modified.
    /// 
    /// Key points for customization:
    /// 1. Change class name from BonusGameView to YourModuleNameView
    /// 2. Update namespace Modules.Base.BonusGameModule.Scripts match your module location
    /// 3. Add your specific UI elements and commands
    /// 4. Customize event handling for your UI
    /// 5. Update validation methods for your UI elements
    /// 6. Add any additional UI functionality your module needs
    /// </summary>
    public class BonusGameView : BaseView
    {
        [Header("UI Elements")]
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button settingsPopupButton;
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private TMP_Text bonusGameScreenTitle;
        
        private InputSystemService _inputSystemService;

        [Inject]
        private void Construct(InputSystemService inputSystemService)
        {
            _inputSystemService = inputSystemService;
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            #if UNITY_EDITOR
            ValidateUIElements();
            #endif
        }

        public void SetupEventListeners(BonusGameCommands commands)
        {
            _inputSystemService.SwitchToUI();
            
            mainMenuButton.OnClickAsObservable()
                .Where(_ => IsActive)
                .Subscribe(_ => commands.OpenMainMenuCommand.Execute(default))
                .AddTo(this);

            settingsPopupButton.OnClickAsObservable()
                .Where(_ => IsActive)
                .Subscribe(_ => commands.SettingsPopupCommand.Execute(default))
                .AddTo(this);

            musicToggle.OnValueChangedAsObservable()
                .Where(_ => IsActive)
                .Subscribe(_ => commands.SoundToggleCommand.Execute(musicToggle.isOn))
                .AddTo(this);

            // Keyboard navigation support
            var openMainMenuPerformedObservable =
                _inputSystemService.GetPerformedObservable(_inputSystemService.InputActions.UI.Cancel);

            openMainMenuPerformedObservable
                .Subscribe(_ => commands.OpenMainMenuCommand.Execute(default))
                .AddTo(this);
        }

        public override async UniTask Show()
        {
            await base.Show();
            _inputSystemService.SwitchToUI();
            _inputSystemService.SetFirstSelectedObject(mainMenuButton);
        }

        public void SetTitle(string title)
        {
            if (bonusGameScreenTitle != null)
                bonusGameScreenTitle.text = title;
            else
                Debug.LogWarning("bonusGameScreenTitle is not assigned in the Inspector.");
        }

        public void InitializeSoundToggle(bool isMusicOn) => musicToggle.SetIsOnWithoutNotify(isMusicOn);

        public void OnScreenEnabled()
        {
            _inputSystemService.SetFirstSelectedObject(mainMenuButton);
        }

        private void ValidateUIElements()
        {
            if (mainMenuButton == null) Debug.LogError($"{nameof(mainMenuButton)} is not assigned in {nameof(BonusGameView)}");
            if (settingsPopupButton == null) Debug.LogError($"{nameof(settingsPopupButton)} is not assigned in {nameof(BonusGameView)}");
            if (musicToggle == null) Debug.LogError($"{nameof(musicToggle)} is not assigned in {nameof(BonusGameView)}");
            if (bonusGameScreenTitle == null) Debug.LogError($"{nameof(bonusGameScreenTitle)} is not assigned in {nameof(BonusGameView)}");
        }
    }
}