using Core;
using Core.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.ConverterScreen.Scripts
{
    public class ConverterInstaller : SceneInstaller
    {
        [SerializeField] private ConverterScreenView converterScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(converterScreenView).As<ConverterScreenView>();
            builder.Register<ConverterScreenPresenter>(Lifetime.Singleton);
            builder.Register<ConverterScreenModel>(Lifetime.Singleton);
        }
    }
}