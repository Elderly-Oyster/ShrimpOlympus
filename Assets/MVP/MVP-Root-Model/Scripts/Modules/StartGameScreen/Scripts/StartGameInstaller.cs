using MVP.MVP_Root_Model.Scripts.Core.Views;
using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Modules.StartGameScreen.Scripts
{
    public class StartGameInstaller : MonoBehaviour, ISceneInstaller
    {
        [SerializeField] private StartGameScreenView startGameScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public void RegisterDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(startGameScreenView).As<StartGameScreenView>();
            builder.Register<StartGameScreenPresenter>(Lifetime.Singleton);
            builder.Register<StartGameScreenModel>(Lifetime.Singleton);
        }
    }
}