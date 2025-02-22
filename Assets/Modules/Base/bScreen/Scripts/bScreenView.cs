using CodeBase.Core.Modules;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Base.bScreen.Scripts
{
    public class bScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text bScreenTitle;
        
        private readonly CompositeDisposable _disposables = new();
        
        public void SetupEventListeners(ReactiveCommand<Unit> mainMenuCommand)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => mainMenuCommand.Execute(default))
                .AddTo(_disposables);
        }
        
        public void SetTitle(string title)
        {
            if(bScreenTitle != null)
                bScreenTitle.text = title;
            else
                Debug.LogWarning("bScreenTitle is not assigned in the Inspector.");
        }

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners() => _disposables.Clear();
    }
}