using Core.MVP;
using R3;
using UnityEngine.UI;
using UnityEngine;

namespace Modules.Base.NewBaseScreen
{
    public class NewModuleScreenView : BaseScreenView
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public void SetupEventListeners(ReactiveCommand<Unit> mainMenuCommand)
        {
            mainMenuButton.onClick.AddListener(() => mainMenuCommand.Execute(Unit.Default));
        }

        public void ResetView() { }

        private void RemoveEventListeners()
        {
            mainMenuButton.onClick.RemoveAllListeners();
        }

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }
    }
}