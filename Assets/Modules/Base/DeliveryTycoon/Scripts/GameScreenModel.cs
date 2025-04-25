using CodeBase.Core.Modules;
using CodeBase.Core.Systems.Save;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using R3;
using Stateless;
using UnityEngine;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GameState
{
    public class GameScreenModel : IScreenModel
    {
        private GameDataSystem _gameDataSystem;
        private SaveSystem _saveSystem;
        
        private ReactiveProperty<GameData> _gameData = new();
        private CompositeDisposable _disposable = new();

        public readonly StateMachine<GameModuleStates, GameModuleStates> StateMachine;
        public ReadOnlyReactiveProperty<GameData> GameData => _gameData;

        [Inject]
        public void Construct(GameDataSystem gameDataSystem, SaveSystem saveSystem)
        {
            _gameDataSystem = gameDataSystem;
            _saveSystem = saveSystem;
            Debug.Log("GameScreenModel Created!");
            SubscribeToEvents();
        }
        
        public GameScreenModel()
        {
            StateMachine = new StateMachine<GameModuleStates, GameModuleStates>(GameModuleStates.Initial);
            ConfigureStateMachine();
            StateMachine.Fire(GameModuleStates.Loading);
        }

        public void ChangeState(GameModuleStates targetStates) => 
            StateMachine.Fire(targetStates);

        private void SubscribeToEvents() => 
            _disposable.Add(_gameDataSystem.GameDataProperty.Subscribe(UpdateGameData));

        private void ConfigureStateMachine()
        {
            StateMachine.Configure(GameModuleStates.Loading)
                .Permit(GameModuleStates.Game, GameModuleStates.Game);
            
            StateMachine.Configure(GameModuleStates.Initial)
                .Permit(GameModuleStates.Loading, GameModuleStates.Loading);
            
            StateMachine.Configure(GameModuleStates.Game).
                Permit(GameModuleStates.UpgradePopup, GameModuleStates.UpgradePopup);
            
            StateMachine.Configure(GameModuleStates.UpgradePopup).
                Permit(GameModuleStates.Game, GameModuleStates.Game);
        }

        private void UpdateGameData(GameData gameData) => 
            _gameData.Value = gameData;

        public void Dispose() => _disposable.Dispose();
    }
}