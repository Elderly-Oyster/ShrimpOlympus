using CodeBase.Core.Modules;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Base.aScreen.Scripts
{
    public class aScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text aScreenTitle;
        
        private readonly CompositeDisposable _disposables = new();
        
        public void SetupEventListeners(ReactiveCommand<Unit> mainMenuCommand)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => mainMenuCommand.Execute(default))
                .AddTo(_disposables);
        }
        
        public void SetTitle(string title)
        {
            if(aScreenTitle != null)
                aScreenTitle.text = title;
            else
                Debug.LogWarning("aScreenTitle is not assigned in the Inspector.");
        }

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners() => _disposables.Clear();
    }
}