using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.RoguelikeScreen.Scripts
{
    public class RoguelikeScreenInstaller : SceneInstaller
    {
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private RoguelikeScreenView roguelikeScreenView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterComponent(roguelikeScreenView).AsImplementedInterfaces().AsSelf();
            builder.Register<RoguelikeStatePresenter>(Lifetime.Singleton);
            builder.Register<RoguelikeScreenModel>(Lifetime.Singleton);
        }
    }
}