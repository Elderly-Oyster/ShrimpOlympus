using CodeBase.Core.Modules;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Base.NewModuleScreen.CodeBase
{
    public class NewModuleScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text newModuleScreenTitle;
        
        private readonly CompositeDisposable _disposables = new();
        
        public void SetupEventListeners(ReactiveCommand<Unit> mainMenuCommand)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => mainMenuCommand.Execute(default))
                .AddTo(_disposables);
        }
        
        public void SetTitle(string title)
        {
            if(newModuleScreenTitle != null)
                newModuleScreenTitle.text = title;
            else
                Debug.LogWarning("newModuleScreenTitle is not assigned in the Inspector.");
        }

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners() => _disposables.Clear();
    }
}