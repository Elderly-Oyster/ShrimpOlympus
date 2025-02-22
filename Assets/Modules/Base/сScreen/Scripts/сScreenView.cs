using CodeBase.Core.Modules;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Base.сScreen.Scripts
{
    public class сScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text сScreenTitle;
        
        private readonly CompositeDisposable _disposables = new();
        
        public void SetupEventListeners(ReactiveCommand<Unit> mainMenuCommand)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => mainMenuCommand.Execute(default))
                .AddTo(_disposables);
        }
        
        public void SetTitle(string title)
        {
            if(сScreenTitle != null)
                сScreenTitle.text = title;
            else
                Debug.LogWarning("сScreenTitle is not assigned in the Inspector.");
        }

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners() => _disposables.Clear();
    }
}