using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;

namespace Modules.Base.ConverterScreen.Scripts
{
    public class ConverterScreenInstaller : BaseModuleSceneInstaller
    {
        [SerializeField] private ConverterView converterView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            base.RegisterSceneDependencies(builder);

            builder.RegisterInstance(converterView).As<ConverterView>();
            builder.Register<ConverterScreenPresenter>(Lifetime.Singleton);
            builder.Register<ConverterModel>(Lifetime.Singleton);
        }
    }
}