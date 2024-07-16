using MVP.MVP_Root_Model.Scripts.Core.Views;
using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuInstaller : MonoBehaviour, ISceneInstaller
    {
        [SerializeField] private MainMenuScreenView mainMenuScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public void RegisterDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(mainMenuScreenView).As<MainMenuScreenView>();
            builder.Register<MainMenuScreenPresenter>(Lifetime.Singleton);
            builder.Register<MainMenuScreenModel>(Lifetime.Singleton);
        }
    }
}