using CodeBase.Core.Modules;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Template.TemplateScreen.Scripts
{
    public class TemplateScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text templateScreenTitle;
        
        private readonly CompositeDisposable _disposables = new();
        
        
        public void SetupEventListeners(ReactiveCommand<Unit> mainMenuCommand)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => mainMenuCommand.Execute(default))
                .AddTo(_disposables);
        }
        
        public void SetTitle(string title)
        {
            if(templateScreenTitle != null)
                templateScreenTitle.text = title;
            else
                Debug.LogWarning("templateScreenTitle is not assigned in the Inspector.");
        }
        
        private void RemoveEventListeners() => _disposables.Clear();

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }
    }
}