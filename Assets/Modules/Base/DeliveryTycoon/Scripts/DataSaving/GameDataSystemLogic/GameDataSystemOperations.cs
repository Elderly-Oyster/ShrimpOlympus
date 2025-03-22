using MediatR;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem
{
    public class GameDataSystemOperations
    {
        public record LoadDataCommand(GameData.TycoonData TycoonData) : IRequest
        {
            public GameData.TycoonData TycoonData { get; } = TycoonData;
        }
    }
}