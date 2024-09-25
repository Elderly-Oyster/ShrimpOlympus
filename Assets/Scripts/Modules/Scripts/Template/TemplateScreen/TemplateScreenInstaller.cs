using Core.Scripts.Services.SceneInstallerService;
using Core.Scripts.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Scripts.Template.TemplateScreen
{
    public class TemplateInstaller : SceneInstaller
    {
        [SerializeField] private TemplateScreenView templateScreenView;
        [SerializeField] private ScreensCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(templateScreenView).As<TemplateScreenView>();
            builder.Register<TemplatePresenter>(Lifetime.Singleton);
            builder.Register<TemplateScreenModel>(Lifetime.Singleton);
        }
    }
}