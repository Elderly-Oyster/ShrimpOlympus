using MVP_0.MVP_Root_Model.Scripts.Core;
using MVP_0.MVP_Root_Model.Scripts.Core.Views;
using MVP_0.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts;
using MVP_0.MVP_Root_Model.Scripts.Modules.StartGame.Scripts;
using MVP_0.MVP_Root_Model.Scripts.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP_0.MVP_Root_Model.Scripts.Startup
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private ScreenTypeController screenTypeController;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;
        
        [SerializeField] private StartGameScreenView startGameScreenView;
        [SerializeField] private ConverterScreenView converterScreenView;

        protected override void Configure(IContainerBuilder builder)
        {            
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);
            
            builder.Register<FirstLongInitializationService>(Lifetime.Singleton);
            builder.Register<SecondLongInitializationService>(Lifetime.Singleton);
            builder.Register<ThirdLongInitializationService>(Lifetime.Singleton);

            builder.Register<StartGameScreenPresenter>(Lifetime.Transient);
            builder.Register<ConverterScreenPresenter>(Lifetime.Transient);
            builder.Register<ConverterScreenModel>(Lifetime.Singleton);
            builder.Register<StartGameScreenModel>(Lifetime.Singleton); // Чуть позже
            builder.Register<ScreenTypeMapper>(Lifetime.Singleton);
            
            builder.RegisterInstance(screenTypeController).As<IRootController>();
            builder.RegisterInstance(startGameScreenView).As<StartGameScreenView>();
            
            builder
                .RegisterComponentInNewPrefab(converterScreenView, Lifetime.Transient)
                .UnderTransform(rootCanvas.transform);
        }
    }
}