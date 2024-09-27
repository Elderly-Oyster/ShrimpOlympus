using Core.Scripts.MVP;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Template.TemplateScreen.Scripts
{
    public class TemplateScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        
        private readonly CompositeDisposable _disposables = new();
        public void SetupEventListeners(ReactiveCommand<Unit> mainMenuCommand)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => mainMenuCommand.Execute(default))
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