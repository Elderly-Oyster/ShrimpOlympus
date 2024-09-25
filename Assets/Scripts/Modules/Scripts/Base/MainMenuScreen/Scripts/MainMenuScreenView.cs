using Core.Scripts.MVP;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Scripts.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenView : BaseScreenView
    {
        [SerializeField] private Button firstPopupButton;
        [SerializeField] private Button secondPopupButton;
        [SerializeField] private Button converterButton;
        [SerializeField] private Button ticTacButton;

        
        protected override void Awake()
        {
            base.Awake();
            HideInstantly();
        }

        public void SetupEventListeners(
            ReactiveCommand<Unit> converterCommand,
            ReactiveCommand<Unit> ticTacCommand,
            ReactiveCommand<Unit> firstPopupCommand,
            ReactiveCommand<Unit> secondPopupCommand)
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
        }
    }
}