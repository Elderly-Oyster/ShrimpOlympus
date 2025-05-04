using System.Collections.Generic;
using CodeBase.Core.Modules.Installer;
using CodeBase.Core.UI;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.NPCCars;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService;
using Modules.Base.DeliveryTycoon.Scripts.GameState;
using Modules.Base.DeliveryTycoon.Scripts.PausePopup;
using Modules.Base.DeliveryTycoon.Scripts.UpgradePopupState;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic.ContainerManagerOperations;


namespace Modules.Base.DeliveryTycoon.Scripts
{
    
    public class GameInstallerForScene : BaseModuleSceneInstaller
    {
        [SerializeField] private GameView gameView;
        [SerializeField] private UpgradePopupView upgradePopupView;
        [SerializeField] private PauseView pauseView;
        [SerializeField] private ContainerManager containerManager;
        [SerializeField] private CarController carController;
        [SerializeField] private ReceiverManager receiverManager;
        [SerializeField] private CarConfig carConfig;
        [SerializeField] private NPCCarManager npcCarManager;
        [SerializeField] private FakeManager fakeManager;
        [SerializeField] private List<ContainerHolder> containerHolders;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            base.RegisterSceneDependencies(builder);
            
            RegisterViews(builder);
            RegisterServices(builder);
            RegisterCarDependencies(builder);
            
            builder.RegisterComponent(receiverManager);
            builder.AddMediatR(typeof(AddNewContainerCommand).Assembly);
            builder.Register<GameManager>(Lifetime.Singleton);
            builder.RegisterComponent(fakeManager);
            
            RegisterMvp(builder);
        }

        private void RegisterViews(IContainerBuilder builder)
        {
            builder.RegisterComponent(gameView).AsImplementedInterfaces().AsSelf();
            builder.RegisterComponent(upgradePopupView).AsImplementedInterfaces().AsSelf();
            builder.RegisterComponent(pauseView).AsImplementedInterfaces().AsSelf();
        }

        private static void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<GameDataSystem>(Lifetime.Singleton)
                .As<GameDataSystem>();
            
            builder.Register<CurrencyService>(Lifetime.Singleton);
            builder.Register<LevelService>(Lifetime.Singleton);
        }

        private void RegisterCarDependencies(IContainerBuilder builder)
        {
            builder.RegisterInstance(containerHolders);
            builder.RegisterComponent(containerManager);
            
            builder.Register<CarInputs>(Lifetime.Singleton);
            builder.RegisterComponent(carController)
                .AsImplementedInterfaces()
                .As<CarController>();
            
            builder.RegisterInstance(carConfig);
            builder.Register<CarFactory>(Lifetime.Singleton);
            builder.Register<CarPool>(Lifetime.Singleton);
            
            builder.RegisterComponent(npcCarManager)
                .AsImplementedInterfaces()
                .AsSelf();
        }

        private static void RegisterMvp(IContainerBuilder builder)
        {
            builder.Register<GameModuleController>(Lifetime.Singleton);
            builder.Register<GameScreenPresenter>(Lifetime.Singleton);
            builder.Register<UpgradePopupPresenter>(Lifetime.Singleton);
            builder.Register<PauseScreenPresenter>(Lifetime.Singleton);
            builder.Register<GameModuleModel>(Lifetime.Singleton);
        }
    }
}