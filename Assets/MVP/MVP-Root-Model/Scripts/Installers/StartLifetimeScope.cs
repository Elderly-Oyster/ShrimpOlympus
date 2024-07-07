using MVP.MVP_Root_Model.Scripts.Core.Views;
using MVP.MVP_Root_Model.Scripts.Modules.StartGame.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Installers
{
    public class StartLifeTimeScope : LifetimeScope
    {
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private StartGameScreenView startGameScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(startGameScreenView).As<StartGameScreenView>();
            builder.Register<StartGameScreenPresenter>(Lifetime.Singleton);
            builder.Register<StartGameScreenModel>(Lifetime.Singleton);
        }
    }
}