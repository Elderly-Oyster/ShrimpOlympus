using CodeBase.Core.Modules;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenView : BaseScreenView
    {
        [SerializeField] private Button settingsPopupButton;
        [SerializeField] private Button secondPopupButton;
        [SerializeField] private Button converterButton;
        [SerializeField] private Button ticTacButton;
        [SerializeField] private Button tycoonButton;
        [SerializeField] private Toggle musicToggle;

        public Button TycoonButton => tycoonButton;

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
            ReactiveCommand<Unit> settingsPopupCommand,
            ReactiveCommand<Unit> secondPopupCommand,
            ReactiveCommand<bool> soundToggleCommand)
        {
            converterButton.OnClickAsObservable()
                .Subscribe(_ => converterCommand.Execute(default))
                .AddTo(this);

            ticTacButton.OnClickAsObservable()
                .Subscribe(_ => ticTacCommand.Execute(default))
                .AddTo(this);

            tycoonButton.OnClickAsObservable()
                .Subscribe(_ => tycoonCommand.Execute(default)).AddTo(this);

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