using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.ConverterScreen.Scripts
{
    public class ConverterScreenInstaller : BaseModuleSceneInstaller
    {
        [SerializeField] private ConverterView converterView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            base.RegisterSceneDependencies(builder);

            builder.RegisterComponent(converterView).AsSelf();
            builder.Register<ConverterScreenPresenter>(Lifetime.Singleton);
            builder.Register<ConverterModel>(Lifetime.Singleton);
        }
    }
}