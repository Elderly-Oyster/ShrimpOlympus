using System.Threading;
using CodeBase.Core.Modules;
using CodeBase.Core.UI.ProgressBars;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class GameScreenView : BaseScreenView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button upgradePopupButton;
        [SerializeField] private TMP_Text playerMoneyText;
        [SerializeField] private BaseProgressBarView experienceProgressBar;
        [SerializeField] private TMP_Text levelText;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        public void SetupEventListeners(ReactiveCommand<Unit> onMainMenuButtonClicked, ReactiveCommand<Unit> onUpgradePopupButtonClicked)
        {
            mainMenuButton.OnClickAsObservable()
                .Subscribe(_ => onMainMenuButtonClicked.Execute(default))
                .AddTo(this);
            upgradePopupButton.OnClickAsObservable()
                .Subscribe(_ => onUpgradePopupButtonClicked.Execute(default))
                .AddTo(this);
        }

        public void InitializeVisualElements( int playerMoney, int level)
        {
            UpdatePlayerMoney(playerMoney);
            experienceProgressBar.AnimateToZero(0, experienceProgressBar.CurrentRatio).Forget();
            UpdateLevel(level);
        }
        
        public void UpdatePlayerMoney(int playerMoney) => 
            playerMoneyText.text = "Money  " + playerMoney;

        public void UpdateExperience(float experience) => 
            experienceProgressBar.Animate(0.5f, CancellationToken.None, experience).Forget();

        public void UpdateLevel(int level) =>
            levelText.text = "Level " + level;

        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners()
        {
            mainMenuButton.onClick.RemoveAllListeners();
        }
    }
}