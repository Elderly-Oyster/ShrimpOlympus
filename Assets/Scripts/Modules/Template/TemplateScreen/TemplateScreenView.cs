using Core.MVP;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Template.TemplateScreen
{
    public class TemplateScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        public void SetupEventListeners(ReactiveCommand mainMenuCommand)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => mainMenuCommand.Execute())
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