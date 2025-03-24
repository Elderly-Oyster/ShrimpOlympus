using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.Save;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService;
using R3;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class GameScreenPresenter : IScreenPresenter
    {
        private readonly LevelService _levelService;
        private readonly GameScreenModel _screenModel;
        private readonly GameScreenView _screenView;
        private readonly TaskCompletionSource<bool> _screenCompletionSource;
        private readonly CurrencyService _currencyService;
        private readonly GameManager _gameManager;
        private readonly GameDataSystem _gameDataSystem;
        private readonly AudioSystem _audioSystem;
        private readonly SaveSystem _saveSystem;
        private readonly LoadingServiceProvider _loadingServiceProvider;
        private CompositeDisposable _disposables = new();
        
        private readonly ReactiveCommand<ScreenPresenterMap> _onMainMenuButtonClicked = new();
        private readonly ReactiveCommand<Unit> _onMainMenuButtonClickedCommand = new();
        private readonly ReactiveCommand<Unit> _onUpgradePopupButtonClickedCommand = new();
        public ReactiveCommand<ScreenPresenterMap> OnMainMenuButtonClickedCommand => _onMainMenuButtonClicked;
        
        public GameScreenPresenter( GameScreenModel screenModel, 
            GameScreenView screenView, LevelService levelService,
            AudioSystem audioSystem, GameDataSystem gameDataSystem,
            GameManager gameManager, SaveSystem saveSystem, CurrencyService currencyService, LoadingServiceProvider loadingServiceProvider)
        {
            _screenModel = screenModel;
            _screenView = screenView;
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
            _screenView.HideInstantly();
            
            await _gameDataSystem.DataLoaded.Task;
            _gameManager.SendGameData(_gameDataSystem.GameDataProperty.CurrentValue);
            _loadingServiceProvider.CompleteRegistration();
            
            SubscribeToReactiveEvents();
            
            var musicVolume = _audioSystem.MusicVolume;
            if (musicVolume > 0) 
                _audioSystem.PlayGameMelody();
            
            _screenView.SetupEventListeners
            (
               _onMainMenuButtonClickedCommand,
               _onUpgradePopupButtonClickedCommand
               
            );
            await _screenView.Show();
            _screenView.InitializeVisualElements(_gameDataSystem.GetMoneyData(), _gameDataSystem.GetLevelData());
        }

        public async UniTask Execute() => await _screenCompletionSource.Task;

        public async UniTask Exit()
        {
            _saveSystem.SaveData().Forget();
            _screenCompletionSource.TrySetResult(true);
            await _screenView.Hide();
        }

        public async void ShowGameScreenView() => await _screenView.Show();

        private void SubscribeToReactiveEvents()
        {
            _disposables.Add(_currencyService.Money.
                Subscribe(UpdateMoneyCounter));
            _disposables.Add(_levelService.Level.
                Subscribe(UpdateLevelText));
            _disposables.Add(_levelService.ExperienceForProgressBar.
                Subscribe(UpdateExperienceProgressBar));
        }

        private void SubscribeToUIUpdates()
        {
            _onMainMenuButtonClickedCommand.Subscribe(_ => OnMainMenuButtonClicked());
            _onUpgradePopupButtonClickedCommand.Subscribe(_ => OnUpgradePopupButtonClicked());
        }
        
        private void UpdateMoneyCounter(int money) => _screenView.UpdatePlayerMoney(money);
        
        private void UpdateLevelText(int level) => _screenView.UpdateLevel(level);
        
        private void UpdateExperienceProgressBar(float experience) => 
            _screenView.UpdateExperience(experience);


        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ScreenPresenterMap.MainMenu);

        private async void OnUpgradePopupButtonClicked()
        {
            _screenModel.ChangeState(GameScreenState.UpgradePopup);
            await _screenView.Hide();
        }

        private void RunNewScreen(ScreenPresenterMap screen) => 
            _onMainMenuButtonClicked.Execute(screen);

        public void Dispose()
        {
            _screenView.Dispose();
            _screenModel.Dispose();
        }
    }
}
