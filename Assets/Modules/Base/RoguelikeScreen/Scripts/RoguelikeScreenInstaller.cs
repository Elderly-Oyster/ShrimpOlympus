using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using MediatR;
using Modules.Base.RoguelikeScreen.Scripts.Character;
using Modules.Base.RoguelikeScreen.Scripts.Enemy;
using Modules.Base.RoguelikeScreen.Scripts.MediatorHandle;
using Modules.Base.RoguelikeScreen.Scripts.Projectile;
using Modules.Base.RoguelikeScreen.Scripts.Service;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.RoguelikeScreen.Scripts
{
    public class RoguelikeScreenInstaller : SceneInstaller
    {
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private RoguelikeScreenView roguelikeScreenView;
        [Header("Bullet")] [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private Transform bulletParent;
        [Header("Character")] [SerializeField] private CharacterView characterView;
        [Header("Enemy")] [SerializeField] private EnemyView enemyPrefab;
        [SerializeField] private Transform enemyParent;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);
            builder.RegisterComponent(mainCamera.gameObject.GetComponent<CameraFollow>());

            builder.RegisterComponent(roguelikeScreenView).AsImplementedInterfaces().AsSelf();
            builder.Register<RoguelikeStatePresenter>(Lifetime.Singleton).AsSelf().As<
                INotificationHandler<PlayerDeathNotification>,
                INotificationHandler<EnemyDeathNotification>>();
            builder.Register<RoguelikeScreenModel>(Lifetime.Singleton);

            RegisterBullet(builder);
            RegisterCharacter(builder);
            RegisterEnemy(builder);
            RegisterMediatR(builder);
        }

        private void RegisterMediatR(IContainerBuilder builder)
        {
            builder.Register(resolver =>
            {
                var mediator = new Mediator(resolver.Resolve);
                return mediator;
            }, Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }

        private void RegisterBullet(IContainerBuilder builder)
        {
            builder.Register(resolver =>
                new BulletFactory(resolver, bulletPrefab), Lifetime.Singleton);
            builder.Register(resolver => new BulletMemoryPool(resolver,
                resolver.Resolve<BulletFactory>(), bulletParent, 5), Lifetime.Singleton);
        }

        private void RegisterCharacter(IContainerBuilder builder)
        {
            builder.RegisterComponent(characterView).AsSelf();
            builder.Register<CharacterPresenter>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<CharacterModel>(Lifetime.Singleton).AsSelf();
        }

        private void RegisterEnemy(IContainerBuilder builder)
        {
            builder.Register(resolver =>
            {
                var view = resolver.Instantiate(enemyPrefab);
                var model = new EnemyModel();

                var bulletPool = resolver.Resolve<BulletMemoryPool>();
                var enemyPool = resolver.Resolve<EnemyMemoryPool>();
                var charModel = resolver.Resolve<CharacterModel>();
                var mediator = resolver.Resolve<Mediator>();

                var enemyPresenter = new EnemyPresenter(model, view, bulletPool, enemyPool, charModel, mediator);
                builder.RegisterInstance(enemyPresenter).AsImplementedInterfaces();
                return view;
            }, Lifetime.Transient);

            builder.Register(resolver =>
                new EnemyMemoryPool(resolver, new EnemyFactory(resolver), enemyParent), Lifetime.Singleton);

            builder.Register(resolver =>
            {
                return new EnemySpawner(5, 3,
                    resolver.Resolve<EnemyMemoryPool>(), resolver.Resolve<CharacterModel>(),
                    seconds => (int)(seconds / 30) + 3);
            }, Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}