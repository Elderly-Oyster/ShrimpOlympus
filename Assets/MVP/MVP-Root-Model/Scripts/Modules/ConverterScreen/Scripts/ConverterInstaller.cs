using MVP.MVP_Root_Model.Scripts.Core.Views;
using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts
{
    public class ConverterInstaller : MonoBehaviour, ISceneInstaller
    {
        [SerializeField] private ConverterScreenView converterScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public LifetimeScope CreateSceneLifetimeScope(LifetimeScope currentScope)
        {
            LifetimeScope instantScope = currentScope.CreateChild(builder =>
            {
                builder.RegisterComponent(rootCanvas);
                builder.RegisterInstance(mainCamera);

                builder.RegisterInstance(converterScreenView).As<ConverterScreenView>();
                builder.Register<ConverterScreenPresenter>(Lifetime.Singleton);
                builder.Register<ConverterScreenModel>(Lifetime.Singleton);
            });
            return instantScope;
        }
    }
}