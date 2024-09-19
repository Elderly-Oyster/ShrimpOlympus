using Core;
using Core.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.NewBaseScreen
{
    public class TemplateInstaller : SceneInstaller
    {
        [SerializeField] private TemplateView newModuleScreenView;
        [SerializeField] private ScreensCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(newModuleScreenView).As<TemplateView>();
            builder.Register<TemplatePresenter>(Lifetime.Singleton);
            builder.Register<TemplateModel>(Lifetime.Singleton);
        }
    }
}