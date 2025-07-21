using System.Threading;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Core.Systems.Save;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using R3;
using Stateless;
using UnityEngine;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class GameModel : IModel
    {
        private GameDataSystem _gameDataSystem;
        private SaveSystem _saveSystem;
        
        private readonly ReactiveProperty<GameData> _gameData = new();
        private CompositeDisposable _disposable = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1); 
        private InputSystemService _inputSystemService;

        public readonly StateMachine<GameModuleStates, GameModuleStates> StateMachine;
        public ReadOnlyReactiveProperty<GameData> GameData => _gameData;

        [Inject]
        public void Construct(GameDataSystem gameDataSystem, 
            SaveSystem saveSystem, InputSystemService inputSystemService)
        {
            _gameDataSystem = gameDataSystem;
            _saveSystem = saveSystem;
            _inputSystemService = inputSystemService;
            Debug.Log("GameModuleModel Created!");
            SubscribeToEvents();
        }
        
        public GameModel()
        {
            StateMachine = new StateMachine<GameModuleStates, GameModuleStates>(GameModuleStates.Initial);
            ConfigureStateMachine();
        }

        public async UniTask ChangeState(GameModuleStates targetStates)
        {
            _inputSystemService.InputActions.Disable();
            await _semaphoreSlim.WaitAsync();
            await StateMachine.FireAsync(targetStates);
        }

        private void SubscribeToEvents() => 
            _disposable.Add(_gameDataSystem.GameDataProperty.Subscribe(UpdateGameData));

        private void ConfigureStateMachine()
        {
            StateMachine.Configure(GameModuleStates.Initial)
                .Permit(GameModuleStates.Loading, GameModuleStates.Loading);
            
            StateMachine.Configure(GameModuleStates.Loading)
                .Permit(GameModuleStates.Game, GameModuleStates.Game);

            StateMachine.Configure(GameModuleStates.Game).
                Permit(GameModuleStates.UpgradePopup, GameModuleStates.UpgradePopup)
                .Permit(GameModuleStates.Pause, GameModuleStates.Pause);
            
            StateMachine.Configure(GameModuleStates.UpgradePopup).
                Permit(GameModuleStates.Game, GameModuleStates.Game);
            
            StateMachine.Configure(GameModuleStates.Pause)
                .Permit(GameModuleStates.Game, GameModuleStates.Game);
        }
        
        public void ReleaseSemaphoreSlim() => _semaphoreSlim.Release();

        private void UpdateGameData(GameData gameData) => 
            _gameData.Value = gameData;

        public void Dispose() => _disposable.Dispose();
    }
}