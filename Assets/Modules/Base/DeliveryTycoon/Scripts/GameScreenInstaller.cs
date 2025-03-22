using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.NPCCars;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManager;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService;
using Modules.Base.DeliveryTycoon.Scripts.UpgradePopup;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManager.ContainerManagerOperations;


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
            builder.AddMediatR(typeof(AddNewContainerCommand).Assembly);
            builder.Register<GameDataSystem>(Lifetime.Singleton).As<GameDataSystem>();
            builder.Register<LevelService>(Lifetime.Singleton);
            builder.Register<CurrencyService>(Lifetime.Singleton);
            builder.RegisterComponent(containerManager);
            builder.RegisterComponent(carController).AsImplementedInterfaces().As<CarController>();
            builder.RegisterInstance(carConfig);
            builder.Register<CarFactory>(Lifetime.Singleton);
            builder.Register<CarPool>(Lifetime.Singleton);
            builder.RegisterComponent(npcCarManager).AsImplementedInterfaces().AsSelf();
            builder.RegisterInstance(receiverManager);
            builder.Register<GameManager>(Lifetime.Singleton);
            
            builder.Register<GameScreenController>(Lifetime.Singleton);
            builder.Register<GameScreenPresenter>(Lifetime.Singleton);
            builder.Register<UpgradePopupPresenter>(Lifetime.Singleton);
            builder.Register<GameScreenModel>(Lifetime.Singleton);
        }
    }
}