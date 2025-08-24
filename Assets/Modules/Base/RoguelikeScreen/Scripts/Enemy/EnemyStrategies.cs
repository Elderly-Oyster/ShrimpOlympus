using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes;
using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Strategies;
using Modules.Base.RoguelikeScreen.Scripts.Projectile;
using R3;
using UnityEngine;

namespace Modules.Base.RoguelikeScreen.Scripts.Enemy
{
    public class DistanceChecker : ICondition
    {
        private readonly ReadOnlyReactiveProperty<Vector2> _positionA;
        private readonly ReadOnlyReactiveProperty<Vector2> _positionB;
        private readonly float _requiredDistance;

        private readonly ConditionStrategy _conditionStrategy;

        public DistanceChecker(ReadOnlyReactiveProperty<Vector2> positionA, 
            ReadOnlyReactiveProperty<Vector2> positionB, float requiredDistance)
        {
            _positionA = positionA;
            _positionB = positionB;
            _requiredDistance = requiredDistance;
        }
        
        public bool Check()
        {
            float distance = Vector2.Distance(_positionA.CurrentValue, _positionB.CurrentValue);
            return distance <= _requiredDistance;
        }
    }

    public class ShootStrategy : IStrategy
    {
        private readonly ReadOnlyReactiveProperty<Vector2> _characterPosition;
        private readonly EnemyView _view;
        private readonly BulletMemoryPool _bulletMemoryPool;

        public ShootStrategy(ReadOnlyReactiveProperty<Vector2> characterPosition, 
            EnemyView view, BulletMemoryPool bulletMemoryPool)
        {
            _characterPosition  = characterPosition;
            _view = view;
            _bulletMemoryPool = bulletMemoryPool;
        }

        public Node.Status Process()
        {
            _bulletMemoryPool.Spawn(_view.GunPosition, _characterPosition.CurrentValue, BulletType.Enemy);
            return Node.Status.Success;
        }

        public void Reset()
        {
        }
    }

    public class ChaseStrategy : IStrategy
    {
        private readonly EnemyPresenter _presenter;
        private readonly ReadOnlyReactiveProperty<Vector2> _characterPosition;

        public ChaseStrategy(EnemyPresenter presenter, ReadOnlyReactiveProperty<Vector2> characterPosition)
        {
            _presenter = presenter;
            _characterPosition = characterPosition;
        }

        public Node.Status Process()
        {
            _presenter.Follow(_characterPosition.CurrentValue);
            return Node.Status.Success;
        }

        public void Reset()
        {
        }
    }

    public class WanderStrategy : IStrategy
    {
        private readonly float _pointPickMinRange;
        private readonly float _pointPickMaxRange;
        private readonly ReadOnlyReactiveProperty<Vector2> _currentPosition;
        private readonly EnemyPresenter _presenter;

        private Vector2 _point;
        private Vector2 _direction;

        public WanderStrategy(ReadOnlyReactiveProperty<Vector2> currentPosition, float pointPickMinRange,
            float pointPickMaxRange, EnemyPresenter presenter)
        {
            _currentPosition = currentPosition;
            _pointPickMinRange = pointPickMinRange;
            _pointPickMaxRange = pointPickMaxRange;
            _presenter = presenter;
        }
        
        public Node.Status Process() =>
            _presenter.FollowAndCheckRange(_point);
        
        public void Reset() =>
            _point = GetRandomPointInRing(_pointPickMinRange, _pointPickMaxRange);
        
        private Vector2 GetRandomPointInRing(float minRadius, float maxRadius)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minRadius, maxRadius);
            return _currentPosition.CurrentValue + randomDirection * randomDistance;
        }
    }
}