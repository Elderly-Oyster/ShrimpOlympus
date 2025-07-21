using CodeBase.Core.UI.Views;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts
{
    public class RoguelikeScreenView : BaseView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text roguelikeScreenTitle;
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
            if (roguelikeScreenTitle)
                roguelikeScreenTitle.text = title;
            else
                Debug.LogWarning("roguelikeScreenTitle is not assigned in the Inspector.");
        }
    }
}