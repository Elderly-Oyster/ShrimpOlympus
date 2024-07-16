using MVP.MVP_Root_Model.Scripts.Core.Views;
using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Modules.TicTacScreen.Scripts
{
    public class TicTacInstaller : MonoBehaviour, ISceneInstaller
    {
        [SerializeField] private TicTacScreenView ticTacScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public LifetimeScope CreateSceneLifetimeScope(LifetimeScope currentScope)
        {
            LifetimeScope instantScope = currentScope.CreateChild(builder =>
            {
                builder.RegisterComponent(rootCanvas);
                builder.RegisterInstance(mainCamera);

                builder.RegisterInstance(ticTacScreenView).As<TicTacScreenView>();
                builder.Register<TicTacScreenPresenter>(Lifetime.Singleton);
                builder.Register<TicTacScreenModel>(Lifetime.Singleton);
            });
            return instantScope;
        }
    }
}