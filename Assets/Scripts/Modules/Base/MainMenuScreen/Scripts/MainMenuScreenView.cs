using Core.MVP;
using UniRx;
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

        
        protected override void Awake()
        {
            base.Awake();
            HideInstantly();
        }

        public void SetupEventListeners(
            ReactiveCommand converterCommand,
            ReactiveCommand ticTacCommand,
            ReactiveCommand firstPopupCommand,
            ReactiveCommand secondPopupCommand)
        {
            converterButton.OnClickAsObservable()
                .Subscribe(_ => converterCommand.Execute())
                .AddTo(this);

            ticTacButton.OnClickAsObservable()
                .Subscribe(_ => ticTacCommand.Execute())
                .AddTo(this);

            firstPopupButton.OnClickAsObservable()
                .Subscribe(_ => firstPopupCommand.Execute())
                .AddTo(this);

            secondPopupButton.OnClickAsObservable()
                .Subscribe(_ => secondPopupCommand.Execute())
                .AddTo(this);
        }
    }
}