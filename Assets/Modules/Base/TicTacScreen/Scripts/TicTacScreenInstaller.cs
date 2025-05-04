using CodeBase.Core.Modules.Installer;
using UnityEngine;
using VContainer;

namespace Modules.Base.TicTacScreen.Scripts
{
    public class TicTacScreenInstaller : BaseModuleSceneInstaller
    {
        [SerializeField] private TicTacView ticTacView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            base.RegisterSceneDependencies(builder);

            builder.RegisterInstance(ticTacView).As<TicTacView>();
            builder.Register<TicTacScreenPresenter>(Lifetime.Singleton);
            builder.Register<TicTacModuleModel>(Lifetime.Singleton);
        }
    }
}