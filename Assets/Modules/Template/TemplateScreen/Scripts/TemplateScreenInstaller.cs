using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Template.TemplateScreen.Scripts
{
    public class TemplateInstaller : SceneInstaller
    {
        [SerializeField] private TemplateScreenView templateScreenView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(templateScreenView).As<TemplateScreenView>();
            builder.Register<TemplatePresenter>(Lifetime.Singleton);
            builder.Register<TemplateScreenModel>(Lifetime.Singleton);
        }
    }
}