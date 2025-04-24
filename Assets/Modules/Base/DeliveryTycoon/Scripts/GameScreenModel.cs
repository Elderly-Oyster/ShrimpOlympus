using CodeBase.Core.Modules;
using CodeBase.Core.Systems.Save;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using R3;
using Stateless;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class GameScreenModel : IScreenModel
    {
        private GameDataSystem _gameDataSystem;
        private SaveSystem _saveSystem;
        
        private ReactiveProperty<GameData> _gameData = new();
        private CompositeDisposable _disposable = new();

        public readonly StateMachine<GameScreenStates, GameScreenStates> StateMachine;
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
            StateMachine = new StateMachine<GameScreenStates, GameScreenStates>(GameScreenStates.Initial);
            ConfigureStateMachine();
            StateMachine.Fire(GameScreenStates.Loading);
        }

        public void ChangeState(GameScreenStates targetStates) => 
            StateMachine.Fire(targetStates);

        private void SubscribeToEvents() => 
            _disposable.Add(_gameDataSystem.GameDataProperty.Subscribe(UpdateGameData));

        private void ConfigureStateMachine()
        {
            StateMachine.Configure(GameScreenStates.Loading)
                .Permit(GameScreenStates.Game, GameScreenStates.Game);
            
            StateMachine.Configure(GameScreenStates.Initial)
                .Permit(GameScreenStates.Loading, GameScreenStates.Loading);
            
            StateMachine.Configure(GameScreenStates.Game).
                Permit(GameScreenStates.UpgradePopup, GameScreenStates.UpgradePopup);
            
            StateMachine.Configure(GameScreenStates.UpgradePopup).
                Permit(GameScreenStates.Game, GameScreenStates.Game);
        }

        private void UpdateGameData(GameData gameData) => 
            _gameData.Value = gameData;

        public void Dispose() => _disposable.Dispose();
    }
}