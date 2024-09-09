using System;
using CodeBase.Core.MVVM.View;
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
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        
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
                .AddTo(_disposables);

            ticTacButton.OnClickAsObservable()
                .Subscribe(_ => onTicTacButtonClicked())
                .AddTo(_disposables);
            
            firstPopupButton.OnClickAsObservable()
                .Subscribe(_ => onFirstPopupButtonClicked())
                .AddTo(_disposables);

            secondPopupButton.OnClickAsObservable()
                .Subscribe(_ => onSecondPopupButtonClicked())
                .AddTo(_disposables);
        }

        private void RemoveEventListeners() => _disposables.Clear();

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }
    }
}