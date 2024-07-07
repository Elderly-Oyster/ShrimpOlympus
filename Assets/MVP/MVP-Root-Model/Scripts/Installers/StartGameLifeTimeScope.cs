using MVP.MVP_Root_Model.Scripts.Core.Views;
using MVP.MVP_Root_Model.Scripts.Modules.StartGame.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Installers
{
    public class StartGameLifetimeScope : LifetimeScope
    {
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private StartGameScreenView startGameScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("Initializing StartGameLifetimeScope");

            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(startGameScreenView).As<StartGameScreenView>();
            builder.Register<StartGameScreenModel>(Lifetime.Singleton);
            builder.Register<StartGameScreenPresenter>(Lifetime.Singleton);

            Debug.Log("StartGameLifetimeScope configured successfully");
        }
    }
}