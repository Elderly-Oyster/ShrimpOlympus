using CodeBase.Core.Modules.Installer;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenInstaller : BaseModuleSceneInstaller
    {
        [SerializeField] private MainMenuView mainMenuView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            base.RegisterSceneDependencies(builder);

            builder.RegisterComponent(mainMenuView).As<MainMenuView>();
            builder.Register<MainMenuScreenPresenter>(Lifetime.Singleton);
            builder.Register<MainMenuModuleModel>(Lifetime.Singleton);
        }
    }
}