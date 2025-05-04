using CodeBase.Core.Modules.Installer;
using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.ConverterScreen.Scripts
{
    public class ConverterScreenInstaller : SceneInstaller
    {
        [SerializeField] private ConverterView converterView;
        [SerializeField] private BaseScreenCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(converterView).As<ConverterView>();
            builder.Register<ConverterScreenPresenter>(Lifetime.Singleton);
            builder.Register<ConverterModuleModel>(Lifetime.Singleton);
        }
    }
}