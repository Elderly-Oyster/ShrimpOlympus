using MVP.MVP_Root_Model.Scripts.Core.Views;
using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Modules.NewScreen.Scripts
{
    public class NewInstaller : MonoBehaviour, ISceneInstaller
    {
        [SerializeField] private NewScreenView ticTacScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public LifetimeScope CreateSceneLifetimeScope(LifetimeScope currentScope)
        {
            LifetimeScope instantScope = currentScope.CreateChild(builder =>
            {
                builder.RegisterComponent(rootCanvas);
                builder.RegisterInstance(mainCamera);

                builder.RegisterInstance(ticTacScreenView).As<NewScreenView>();
                builder.Register<NewScreenPresenter>(Lifetime.Singleton);
                builder.Register<NewScreenModel>(Lifetime.Singleton);
            });
            return instantScope;
        }
    }
}