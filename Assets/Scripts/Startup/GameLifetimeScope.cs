using Core;
using Core.Views;
using Modules.ConverterScreen.Scripts;
using Modules.StartGame.Scripts;
using Modules.TestMenu.Scripts;
using Services;
using Services.EnergyBar;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Startup
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private ScreenTypeController screenTypeController;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;
        
        [SerializeField] private StartGameScreenView startGameScreenView;
        [SerializeField] private TestMenuUIView testMenuUIView;

        protected override void Configure(IContainerBuilder builder)
        {            
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);
            
            builder.Register<EnergyBarService>(Lifetime.Singleton);

            builder.Register<StartGameScreenPresenter>(Lifetime.Transient);
            builder.Register<TestMenuPresenter>(Lifetime.Transient);
            builder.Register<TestMenuModel>(Lifetime.Singleton);
            builder.Register<StartGameScreenModel>(Lifetime.Singleton); // Чуть позже
            builder.Register<ScreenTypeMapper>(Lifetime.Singleton);
            
            builder.RegisterInstance(screenTypeController).As<IRootController>();
            builder.RegisterInstance(startGameScreenView).As<StartGameScreenView>();
            
            builder
                .RegisterComponentInNewPrefab(testMenuUIView, Lifetime.Transient)
                .UnderTransform(rootCanvas.transform);
        }
    }
}