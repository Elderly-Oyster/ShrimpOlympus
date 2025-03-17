using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    
    public class GameInstallerForScene : SceneInstaller
    {
        [SerializeField] private GameScreenView gameScreenView;
        [SerializeField] private UpgradePopupView upgradePopupView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private ContainerManager containerManager;
        [SerializeField] private CarController carController;
        [SerializeField] private ReceiverManager receiverManager;
        [SerializeField] private CarConfig carConfig;
        [SerializeField] private NPCCarManager npcCarManager;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);
            builder.RegisterInstance(gameScreenView);
            builder.RegisterInstance(upgradePopupView);

            builder.Register<GameDataSystem>(Lifetime.Singleton).As<GameDataSystem>();
            builder.Register<LevelManager>(Lifetime.Singleton);
            builder.RegisterInstance(containerManager);
            builder.RegisterInstance(carController).AsImplementedInterfaces().As<CarController>();
            builder.RegisterInstance(npcCarManager);
            builder.RegisterInstance(carConfig);
            builder.RegisterInstance(receiverManager);
            builder.Register<CarFactory>(Lifetime.Singleton);
            builder.Register<CarPool>(Lifetime.Singleton);
            builder.Register<GameManager>(Lifetime.Singleton);
            
            builder.Register<GameScreenController>(Lifetime.Singleton);
            builder.Register<GameScreenPresenter>(Lifetime.Singleton);
            builder.Register<UpgradePopupPresenter>(Lifetime.Singleton);
            builder.Register<GameScreenModel>(Lifetime.Singleton);
        }
    }
}