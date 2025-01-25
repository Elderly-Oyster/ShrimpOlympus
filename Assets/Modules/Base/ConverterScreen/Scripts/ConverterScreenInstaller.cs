using CodeBase.Core.Root;
using CodeBase.Implementation.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.ConverterScreen.Scripts
{
    public class ConverterScreenInstaller : SceneInstaller
    {
        [SerializeField] private ConverterScreenView converterScreenView;
        [SerializeField] private ScreenCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(converterScreenView).As<ConverterScreenView>();
            builder.Register<ConverterScreenPresenter>(Lifetime.Singleton);
            builder.Register<ConverterScreenModel>(Lifetime.Singleton);
        }
    }
}