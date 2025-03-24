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

        public readonly StateMachine<GameScreenState, GameScreenState> StateMachine;
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
            StateMachine = new StateMachine<GameScreenState, GameScreenState>(GameScreenState.Initial);
            ConfigureStateMachine();
            StateMachine.Fire(GameScreenState.Loading);
        }

        public void ChangeState(GameScreenState targetState) => 
            StateMachine.Fire(targetState);

        private void SubscribeToEvents() => 
            _disposable.Add(_gameDataSystem.GameDataProperty.Subscribe(UpdateGameData));

        private void ConfigureStateMachine()
        {
            StateMachine.Configure(GameScreenState.Loading)
                .Permit(GameScreenState.Game, GameScreenState.Game);
            
            StateMachine.Configure(GameScreenState.Initial)
                .Permit(GameScreenState.Loading, GameScreenState.Loading);
            
            StateMachine.Configure(GameScreenState.Game).
                Permit(GameScreenState.UpgradePopup, GameScreenState.UpgradePopup);
            
            StateMachine.Configure(GameScreenState.UpgradePopup).
                Permit(GameScreenState.Game, GameScreenState.Game);
        }

        private void UpdateGameData(GameData gameData) => 
            _gameData.Value = gameData;

        public void Dispose() => _disposable.Dispose();
    }
}