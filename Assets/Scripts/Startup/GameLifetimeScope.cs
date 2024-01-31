using Core;
using Core.Views;
using Modules.ConverterScreen.Scripts;
using Modules.StartGame.Scripts;
using Services;
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
        
        [SerializeField] private StartGameUIView startGameUIView;
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
            //builder.Register<StartGameModel>(Lifetime.Singleton); //чуть позже
            builder.Register<ScreenTypeMapper>(Lifetime.Singleton);
            
            builder.RegisterInstance(screenTypeController).As<IRootController>();
            builder.RegisterInstance(startGameUIView).As<StartGameUIView>();
            
            builder
                .RegisterComponentInNewPrefab(converterScreenView, Lifetime.Transient)
                .UnderTransform(rootCanvas.transform);
        }
    }
}