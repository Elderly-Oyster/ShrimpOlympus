using CodeBase.Core.UI.Views;
using CodeBase.Services;
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
        [SerializeField] private Button roguelikeButton;
        [SerializeField] private Toggle musicToggle;

        private InputSystemService _inputSystemService;
        
        [Inject]
        public void Construct(InputSystemService inputSystemService) => 
            _inputSystemService = inputSystemService;

        protected override void Awake()
        {
            base.Awake();
            HideInstantly();
        }

        public void Initialize(bool isMusicOn)
        {
            musicToggle.SetIsOnWithoutNotify(isMusicOn);
        }
        
        public void SetupEventListeners(
            ReactiveCommand<Unit> converterCommand,
            ReactiveCommand<Unit> ticTacCommand,
            ReactiveCommand<Unit> tycoonCommand,
            ReactiveCommand<Unit> roguelikeCommand,
            ReactiveCommand<Unit> settingsPopupCommand,
            ReactiveCommand<Unit> secondPopupCommand,
            ReactiveCommand<bool> soundToggleCommand)
        {
            _inputSystemService.SwitchToUI();
            
            converterButton.OnClickAsObservable()
                .Subscribe(_ => converterCommand.Execute(default))
                .AddTo(this);

            ticTacButton.OnClickAsObservable()
                .Subscribe(_ => ticTacCommand.Execute(default))
                .AddTo(this);

            tycoonButton.OnClickAsObservable()
                .Subscribe(_ => tycoonCommand.Execute(default))
                .AddTo(this);
            
            roguelikeButton.OnClickAsObservable()
                .Subscribe(_ => roguelikeCommand.Execute(default))
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
    }
}