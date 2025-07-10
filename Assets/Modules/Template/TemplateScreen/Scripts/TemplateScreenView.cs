using CodeBase.Core.UI.Views;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Modules.Template.TemplateScreen.Scripts
{
    public class TemplateScreenView : BaseView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text templateScreenTitle;
        private InputSystemService _inputSystemService;

        [Inject]
        private void Construct(InputSystemService inputSystemService)
        {
            _inputSystemService = inputSystemService;
        }

        public void SetupEventListeners(ReactiveCommand<Unit> mainMenuCommand)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => mainMenuCommand.Execute(default))
                .AddTo(this);

            var openMainMenuPerformedObservable =
                _inputSystemService.GetPerformedObservable(_inputSystemService.InputActions.UI.Cancel);

            openMainMenuPerformedObservable
                .Subscribe(_ => mainMenuCommand.Execute(default))
                .AddTo(this);
        }

        public override async UniTask Show()
        {
            await base.Show();
            _inputSystemService.SwitchToUI();
            _inputSystemService.SetFirstSelectedObject(mainMenuButton);
        }

        public void SetTitle(string title)
        {
            if (templateScreenTitle != null)
                templateScreenTitle.text = title;
            else
                Debug.LogWarning("templateScreenTitle is not assigned in the Inspector.");
        }
    }
}