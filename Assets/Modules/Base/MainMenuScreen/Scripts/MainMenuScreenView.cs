using CodeBase.Core.Modules;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenView : BaseScreenView
    {
        [SerializeField] private Button firstPopupButton;
        [SerializeField] private Button secondPopupButton;
        [SerializeField] private Button converterButton;
        [SerializeField] private Button ticTacButton;
        [SerializeField] private Toggle musicToggle;
        
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
            ReactiveCommand<Unit> firstPopupCommand,
            ReactiveCommand<Unit> secondPopupCommand,
            ReactiveCommand<bool> soundToggleCommand)
        {
            converterButton.OnClickAsObservable()
                .Subscribe(_ => converterCommand.Execute(default))
                .AddTo(this);

            ticTacButton.OnClickAsObservable()
                .Subscribe(_ => ticTacCommand.Execute(default))
                .AddTo(this);

            firstPopupButton.OnClickAsObservable()
                .Subscribe(_ => firstPopupCommand.Execute(default))
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