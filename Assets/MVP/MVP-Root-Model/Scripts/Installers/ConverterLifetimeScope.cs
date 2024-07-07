using MVP.MVP_Root_Model.Scripts.Core.Views;
using MVP.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Installers
{
    public class ConverterLifetimeScope : LifetimeScope
    {
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        [SerializeField] private ConverterScreenView converterScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.Register<ConverterScreenModel>(Lifetime.Singleton);
            builder.Register<ConverterScreenPresenter>(Lifetime.Singleton);
            builder.RegisterInstance(converterScreenView).As<ConverterScreenView>();
        }
    }
}