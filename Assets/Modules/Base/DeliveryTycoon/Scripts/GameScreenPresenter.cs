using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.Save;
using CodeBase.Services;
using CodeBase.Systems.InputSystem;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class GameScreenPresenter : IScreenPresenter
    {
        private readonly GameScreenModel _screenModel;
        private readonly GameView _view;
        private readonly CurrencyService _currencyService;
        private readonly LevelService _levelService;
        private readonly GameDataSystem _gameDataSystem;
        private readonly AudioSystem _audioSystem;
        private readonly SaveSystem _saveSystem;
        private readonly GameManager _gameManager;
        private readonly LoadingServiceProvider _loadingServiceProvider;
        private readonly TaskCompletionSource<bool> _screenCompletionSource;

        private readonly ReactiveCommand<ScreenPresenterMap> _onMainMenuButtonClicked = new();
        private readonly ReactiveCommand<Unit> _onMainMenuButtonClickedCommand = new();
        private readonly ReactiveCommand<Unit> _onUpgradePopupButtonClickedCommand = new();
        
        private CompositeDisposable _disposables = new();
        
        public ReactiveCommand<ScreenPresenterMap> OnMainMenuButtonClickedCommand => _onMainMenuButtonClicked;

        public GameScreenPresenter( GameScreenModel screenModel, GameView view, LevelService levelService,
            AudioSystem audioSystem, GameDataSystem gameDataSystem, GameManager gameManager, SaveSystem saveSystem,
            CurrencyService currencyService, LoadingServiceProvider loadingServiceProvider)
        {
            _screenModel = screenModel;
            _view = view;
            _levelService = levelService;
            _currencyService = currencyService;
            _loadingServiceProvider = loadingServiceProvider;
            _audioSystem = audioSystem;
            _gameDataSystem = gameDataSystem;
            _gameManager = gameManager;
            _saveSystem = saveSystem;
            _screenCompletionSource = new TaskCompletionSource<bool>();
            
            SubscribeToUIUpdates();
        }

        public async UniTask Enter(object param)
        {
            _view.HideInstantly();
            
            await _gameDataSystem.DataLoaded.Task;
            _gameManager.SendGameData(_gameDataSystem.GameDataProperty.CurrentValue);
            _loadingServiceProvider.CompleteRegistration();
            _gameManager.StartGame();
            SubscribeToReactiveEvents();
            
            var musicVolume = _audioSystem.MusicVolume;
            if (musicVolume > 0) 
                _audioSystem.PlayGameMelody();
            
            _view.SetupEventListeners
            (
               _onMainMenuButtonClickedCommand,
               _onUpgradePopupButtonClickedCommand
            );
            _view.InitializeVisualElements(_gameDataSystem.GetMoneyData(), _gameDataSystem.GetLevelData());
            await _view.Show();
            _loadingServiceProvider.ResetRegistrationProgress();
        }

        public async UniTask Execute() => await _screenCompletionSource.Task;

        public async UniTask Exit()
        {
            _gameManager.EndGame();
            _saveSystem.SaveData().Forget();
            _screenCompletionSource.TrySetResult(true);
            await _view.Hide();
        }

        public async void ShowGameScreenView() => await _view.Show();

        public void OnEscapePressed() => OnMainMenuButtonClicked();

        private void SubscribeToReactiveEvents()
        {
            _disposables.Add(_currencyService.Money.Subscribe(UpdateMoneyCounter));
            _disposables.Add(_levelService.Level.Subscribe(UpdateLevelText));
            _disposables.Add(_levelService.ExperienceForProgressBar.Subscribe(UpdateExperienceProgressBar));
        }

        private void SubscribeToUIUpdates()
        {
            _onMainMenuButtonClickedCommand.Subscribe(_ => OnMainMenuButtonClicked());
            _onUpgradePopupButtonClickedCommand.Subscribe(_ => OnUpgradePopupButtonClicked());
        }
        
        private void UpdateMoneyCounter(int money) => _view.UpdatePlayerMoney(money);
        
        private void UpdateLevelText(int level) => _view.UpdateLevel(level);
        
        private void UpdateExperienceProgressBar(float experience) => 
            _view.UpdateExperience(experience);

        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ScreenPresenterMap.MainMenu);

        private async void OnUpgradePopupButtonClicked()
        {
            _screenModel.ChangeState(GameScreenStates.UpgradePopup);
            await _view.Hide();
        }

        private void RunNewScreen(ScreenPresenterMap screen) => 
            _onMainMenuButtonClicked.Execute(screen);

        public void Dispose()
        {
            _view.Dispose();
            _screenModel.Dispose();
        }
    }
}
