using CodeBase.Core.Modules.Installer;
using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Template.TemplateScreen.Scripts
{
    public class TemplateScreenInstaller : SceneInstaller
    {
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private TemplateScreenView templateScreenView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterComponent(templateScreenView).AsImplementedInterfaces().AsSelf();
            builder.Register<TemplateScreenPresenter>(Lifetime.Singleton);
            builder.Register<TemplateModuleModel>(Lifetime.Singleton);
        }
    }
}