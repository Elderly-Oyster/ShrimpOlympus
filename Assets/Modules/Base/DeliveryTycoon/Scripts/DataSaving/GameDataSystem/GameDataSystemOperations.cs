using MediatR;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem
{
    public class GameDataSystemOperations
    {
        public record DataLoaded(GameData.GameData GameData) : IRequest
        {
            public GameData.GameData GameData { get; } = GameData;
        }
    }
}