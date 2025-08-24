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
        [Header("Stats")]
        [SerializeField] private GameObject stats;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text scoreText;
        [Header("OnDeath")]
        [SerializeField] private GameObject onDeath;
        [SerializeField] private Button restartButton;
        private InputSystemService _inputSystemService;

        [Inject]
        private void Construct(InputSystemService inputSystemService)
        {
            _inputSystemService = inputSystemService;
        }

        public void SetupEventListeners(ReactiveCommand<Unit> mainMenuCommand, ReactiveCommand<Unit> restartCommand)
        {
            restartButton.OnClickAsObservable()
                .Subscribe(_ => restartCommand.Execute(default)).
                AddTo(this);
            
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
            _inputSystemService.SwitchToRoguelikeCharacter();
            _inputSystemService.SetFirstSelectedObject(mainMenuButton);
        }

        public void SetTitle(string title)
        {
            if (roguelikeScreenTitle)
                roguelikeScreenTitle.text = title;
            else
                Debug.LogWarning("roguelikeScreenTitle is not assigned in the Inspector.");
        }
        
        public void SetStatsEnabled(bool statsEnabled) => stats.SetActive(statsEnabled);
        public void SetOnDeathEnabled(bool onDeathEnabled) => onDeath.SetActive(onDeathEnabled);
        public void SetHealth(float health) => healthText.text = $"Health: {health}";
        public void SetScore(int score) => scoreText.text = $"Score: {score}";
        
    }
}