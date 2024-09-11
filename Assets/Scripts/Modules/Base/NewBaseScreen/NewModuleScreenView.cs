using System;
using Core.MVP;
using UniRx;
using UnityEngine.UI;
using UnityEngine;

namespace Modules.Base.NewBaseScreen
{
    public class NewModuleScreenView : BaseScreenView
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public void SetupEventListeners(Action onMainMenuButtonClicked)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => onMainMenuButtonClicked())
                .AddTo(_disposables);
        }

        public void ResetView() { }

        private void RemoveEventListeners() => _disposables.Clear();

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }
    }
}