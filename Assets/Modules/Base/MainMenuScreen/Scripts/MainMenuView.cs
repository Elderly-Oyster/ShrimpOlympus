using CodeBase.Core.UI.Views;
using CodeBase.Services.Input;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuView : BaseView
    {
        [SerializeField] private Button settingsPopupButton;
        [SerializeField] private Button secondPopupButton;
        [SerializeField] private Button converterButton;
        [SerializeField] private Button ticTacButton;
        [SerializeField] private Button tycoonButton;
        [SerializeField] private Toggle musicToggle;

        private InputSystemService _inputSystemService;
        
        [Inject]
        public void Construct(InputSystemService inputSystemService)
        {
            _inputSystemService = inputSystemService;
        }
        
        public void SetupEventListeners(
            ReactiveCommand<Unit> openConverterCommand,
            ReactiveCommand<Unit> openTicTacCommand,
            ReactiveCommand<Unit> openTycoonCommand,
            ReactiveCommand<Unit> settingsPopupCommand,
            ReactiveCommand<Unit> secondPopupCommand,
            ReactiveCommand<bool> soundToggleCommand)
        {
            _inputSystemService.SwitchToUI();
            
            converterButton.OnClickAsObservable()
                .Subscribe(_ => openConverterCommand.Execute(default))
                .AddTo(this);

            ticTacButton.OnClickAsObservable()
                .Subscribe(_ => openTicTacCommand.Execute(default))
                .AddTo(this);

            tycoonButton.OnClickAsObservable()
                .Subscribe(_ => openTycoonCommand.Execute(default))
                .AddTo(this);

            settingsPopupButton.OnClickAsObservable()
                .Subscribe(_ => settingsPopupCommand.Execute(default))
                .AddTo(this);

            secondPopupButton.OnClickAsObservable()
                .Subscribe(_ => secondPopupCommand.Execute(default))
                .AddTo(this);

            musicToggle.OnValueChangedAsObservable()
                .Subscribe(_ => soundToggleCommand.Execute(musicToggle.isOn))
                .AddTo(this);
        }
        
        public override async UniTask Show()
        {
            //Example of logic: Work with tooltips system
            // _controlsTooltipSystem.HideAllTooltips(); 
            await base.Show();
            
            _inputSystemService.SwitchToUI();
            OnScreenEnabled();
        }
        
        public void InitializeSoundToggle(bool isMusicOn) => musicToggle.SetIsOnWithoutNotify(isMusicOn);

        public void OnScreenEnabled()
        {
            _inputSystemService.SetFirstSelectedObject(converterButton);
            
            //Other logic. Showing tooltips for example
            // _controlsTooltipSystem.ShowTooltip(SelectTooltipText, 
            //     _inputSystemService.GetFullActionPath(_inputSystemService.InputActions.UI.Submit));
        }
    }
}