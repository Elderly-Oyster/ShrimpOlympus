using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem
{
    public class GameDataSystemHandler : IRequestHandler<GameDataSystemOperations.DataLoaded>
    {
        private readonly GameManager _gameManager;
        private readonly GameDataSystem _gameDataSystem;

        public GameDataSystemHandler(GameManager gameManager, GameDataSystem gameDataSystem)
        {
            Debug.Log("GameDataSystemHandler was registered");
            _gameManager = gameManager;
            _gameDataSystem = gameDataSystem;
        }
        
        public Task<Unit> Handle(GameDataSystemOperations.DataLoaded request, CancellationToken cancellationToken)
        {
           _gameManager.StartGame(_gameManager.MusicVolume, _gameDataSystem.GameDataProperty.CurrentValue);
           return Task.FromResult(Unit.Value);
        }
    }
}