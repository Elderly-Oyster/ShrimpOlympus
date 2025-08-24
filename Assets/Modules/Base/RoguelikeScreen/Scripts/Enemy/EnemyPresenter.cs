using System.Text;
using MediatR;
using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes;
using Modules.Base.RoguelikeScreen.Scripts.Character;
using Modules.Base.RoguelikeScreen.Scripts.MediatorHandle;
using Modules.Base.RoguelikeScreen.Scripts.Projectile;
using R3;
using UnityEngine;

namespace Modules.Base.RoguelikeScreen.Scripts.Enemy
{
    public class EnemyPresenter
    {
        private readonly EnemyModel _model;
        private readonly EnemyView _view;
        private readonly BehaviourTree _behaviorTree;
        private readonly BulletMemoryPool _bulletMemoryPool;
        private readonly EnemyMemoryPool _enemyMemoryPool;
        private readonly CharacterModel _characterModel;
        private readonly Mediator _mediator;

        public EnemyPresenter(
            EnemyModel model,
            EnemyView view,
            BulletMemoryPool bulletMemoryPool,
            EnemyMemoryPool enemyMemoryPool,
            CharacterModel characterModel,
            Mediator mediator)
        {
            _model = model;
            _view = view;
            _characterModel = characterModel;
            _mediator = mediator;

            _behaviorTree = new BehaviourTree("EnemyAI", autoReset: true);
            _bulletMemoryPool = bulletMemoryPool;
            _enemyMemoryPool = enemyMemoryPool;

            InitBehaviourTree();
            Subscribe();
        }

        private void InitBehaviourTree()
        {
            var mainSelector = new PrioritySelector("MainSelector");
            var characterPosition = _characterModel.Position
                .ToReadOnlyReactiveProperty();

            var shootSequence = new Sequence("ShootSequence", 2);
            var shootLeaf = new ConditionalLeaf("ShootLeaf",
                new ShootStrategy(characterPosition, _view, _bulletMemoryPool),
                new DistanceChecker(characterPosition, _model.Position,
                    EnemyModel.ShootRadius));
            var waitNode = new WaitLeaf("AfterShootWait", 1f);
            shootSequence.AddChild(shootLeaf);
            shootSequence.AddChild(waitNode);

            var chaseLeaf = new ConditionalLeaf("ChaseLeaf",
                new ChaseStrategy(this, characterPosition),
                new DistanceChecker(characterPosition, _model.Position,
                    EnemyModel.ChaseRadius), 1);
            var wanderLeaf = new Leaf("WanderLeaf", new WanderStrategy(
                _model.Position.ToReadOnlyReactiveProperty(),
                EnemyModel.WanderRadiusMin, EnemyModel.WanderRadiusMax, this));

            mainSelector.AddChild(shootSequence);
            mainSelector.AddChild(chaseLeaf);
            mainSelector.AddChild(wanderLeaf);

            _behaviorTree.AddChild(mainSelector);
            _behaviorTree.Reset();
        }

        private void Subscribe()
        {
            _view.Tick += Tick;
            _view.DamageReceived += OnDamageReceived;
            _view.ResetEvent += OnReset;
            _model.Position.Subscribe(OnPositionUpdated);
            _model.Health.Where(hp => hp <= 0)
                .Subscribe(_ => OnDeath());
        }

        private void OnDeath()
        {
            _mediator.Publish(new EnemyDeathNotification(10));
            _enemyMemoryPool.Despawn(_view);
        }

        public void Tick()
        {
            _behaviorTree.Process();
            var sb = new StringBuilder();
            _behaviorTree.PrintDebug(sb);
        }

        #region BehaviourTreeInteractions

        private void Move(Vector2 directionUnnormalized)
        {
            _model.Move(directionUnnormalized);
        }

        public void Follow(Vector2 point)
        {
            var direction = point - _model.Position.Value;
            Move(direction);
        }

        public Node.Status FollowAndCheckRange(Vector2 point)
        {
            Follow(point);

            var travelDistance = EnemyModel.MoveSpeed * Time.deltaTime;
            if (Vector2.Distance(_model.Position.CurrentValue, point) < travelDistance)
            {
                return Node.Status.Success;
            }

            return Node.Status.Running;
        }

        #endregion

        #region MVPInteractions

        private void OnPositionUpdated(Vector2 position)
        {
            _view.transform.position = position;
        }

        private void OnDamageReceived(float damage)
        {
            _model.TakeDamage(damage);
        }

        private void OnReset()
        {
            _model.Position.Value = _view.transform.position;
            _model.Health.Value = EnemyModel.MaxHealth;
        }

        #endregion
    }
}