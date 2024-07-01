using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Core.Views;
using MVP.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts;
using MVP.MVP_Root_Model.Scripts.Modules.StartGame.Scripts;
using MVP.MVP_Root_Model.Scripts.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Startup
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
            
            builder.Register<ScreenTypeMapper>(Lifetime.Singleton);

            RegisterServices(builder);
            
            RegisterPresenters(builder);
            RegisterModels(builder);
            RegisterViews(builder);
            
            builder.RegisterInstance(screenTypeController).As<IRootController>();
        }
        
        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<FirstLongInitializationService>(Lifetime.Singleton);
            builder.Register<SecondLongInitializationService>(Lifetime.Singleton);
            builder.Register<ThirdLongInitializationService>(Lifetime.Singleton);
        }
        
        private void RegisterPresenters(IContainerBuilder builder)
        {
            builder.Register<StartGameScreenPresenter>(Lifetime.Transient);
            builder.Register<ConverterScreenPresenter>(Lifetime.Transient);
        }
        
        private void RegisterModels(IContainerBuilder builder)
        {
            builder.Register<StartGameScreenModel>(Lifetime.Transient);
            builder.Register<ConverterScreenModel>(Lifetime.Transient);
        }
        
        private void RegisterViews(IContainerBuilder builder)
        {
            builder.RegisterInstance(startGameScreenView).As<StartGameScreenView>();
            builder
                .RegisterComponentInNewPrefab(converterScreenView, Lifetime.Transient)
                .UnderTransform(rootCanvas.transform);
        }
    }
}