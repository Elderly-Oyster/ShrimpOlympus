using MediatR;

namespace Modules.Base.RoguelikeScreen.Scripts.MediatorHandle
{
    public struct EnemyDeathNotification : INotification
    {
        public int PointsCount{get; private set;}
        
        public EnemyDeathNotification(int pointsCount) => PointsCount = pointsCount;
    }
}