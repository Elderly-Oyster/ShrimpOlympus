using System.Threading;
using CodeBase.Core.Modules;
using CodeBase.Core.UI.ProgressBars;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GameState
{
    public class GameView : BaseView
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button upgradePopupButton;
        [SerializeField] private TMP_Text playerMoneyText;
        [SerializeField] private BaseProgressBarView experienceProgressBar;
        [SerializeField] private TMP_Text levelText;
        private InputSystemService _inputSystemService;

        [Inject]
        private void Construct(InputSystemService inputSystemService) => 
            _inputSystemService = inputSystemService;

        public void SetupEventListeners(//ReactiveCommand<Unit> openMainMenuCommand, 
            ReactiveCommand<Unit> onUpgradePopupButtonClicked, ReactiveCommand<Unit> pausePopupCommand)
        {
            // mainMenuButton.OnClickAsObservable()
            //     .Subscribe(_ => openMainMenuCommand.Execute(default))
            //     .AddTo(this);

            var cancelPerformedObservable = 
                _inputSystemService.GetPerformedObservable(_inputSystemService.InputActions.UI.Cancel);
            
            cancelPerformedObservable
                .Where(_ => IsActive)
                .Subscribe(_ =>
                {
                    Debug.Log("Game View state is " + IsActive);
                    pausePopupCommand.Execute(default);
                })
                .AddTo(this);
            
            upgradePopupButton.OnClickAsObservable()
                .Subscribe( _ => onUpgradePopupButtonClicked.Execute(default))
                .AddTo(this);

            var upgradePopupObservable =
                _inputSystemService
                    .GetPerformedObservable(_inputSystemService.InputActions.PlayerCar.OpenUpgradePopup);
            
            upgradePopupObservable
                .Where(_ => IsActive)
                .Subscribe(_ => onUpgradePopupButtonClicked.Execute(default))
                .AddTo(this);
        }

        public void InitializeVisualElements(int playerMoney, int level)
        {
            UpdatePlayerMoney(playerMoney);
            experienceProgressBar.AnimateToZero(0, experienceProgressBar.CurrentRatio).Forget();
            UpdateLevel(level);
        }
        
        public void UpdatePlayerMoney(int playerMoney) => playerMoneyText.text = "Money  " + playerMoney;

        public void UpdateLevel(int level) => levelText.text = "Level " + level;

        public void UpdateExperience(float experience) => 
            experienceProgressBar.Animate(0.5f, CancellationToken.None, experience).Forget();

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