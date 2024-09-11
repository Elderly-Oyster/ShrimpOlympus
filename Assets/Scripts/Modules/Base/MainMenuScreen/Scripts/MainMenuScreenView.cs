using System;
using CodeBase.Core.MVVM.View;
using Core.UniRx;
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

        private readonly DisposableEntity _disposableEntity = new DisposableEntity();

        
        protected override void Awake()
        {
            base.Awake();
            HideInstantly();
        }

        public void SetupEventListeners(
            Action onConverterButtonClicked,
            Action onTicTacButtonClicked,
            Action onFirstPopupButtonClicked,
            Action onSecondPopupButtonClicked)
        {
            converterButton.OnClickAsObservable()
                .Subscribe(_ => onConverterButtonClicked())
                .AddTo(_disposableEntity);

            ticTacButton.OnClickAsObservable()
                .Subscribe(_ => onTicTacButtonClicked())
                .AddTo(_disposableEntity);

            firstPopupButton.OnClickAsObservable()
                .Subscribe(_ => onFirstPopupButtonClicked())
                .AddTo(_disposableEntity);

            secondPopupButton.OnClickAsObservable()
                .Subscribe(_ => onSecondPopupButtonClicked())
                .AddTo(_disposableEntity);
        }

        private void RemoveEventListeners() => _disposableEntity.Dispose();

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }
    }
}