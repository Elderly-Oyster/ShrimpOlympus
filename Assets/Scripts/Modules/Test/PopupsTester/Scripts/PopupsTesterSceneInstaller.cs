using Core;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using VContainer.Unity;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneInstaller : SceneInstaller
    {
        [SerializeField] private PopupsTesterSceneView popupsTesterSceneView;
        [SerializeField] private TestButtonView buttonPrefab;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterInstance(popupsTesterSceneView).As<PopupsTesterSceneView>();
            builder.Register<PopupsTesterScenePresenter>(Lifetime.Singleton);
            builder.Register<PopupsTesterSceneModel>(Lifetime.Singleton)
                .As<IStartable>()
                .AsSelf();
            
            builder.RegisterFactory<UnityAction, TestButtonView>(action =>
            {
                var testButton = Instantiate(buttonPrefab, popupsTesterSceneView.buttonsParent);
                testButton.gameObject.SetActive(true);
                testButton.label.text = action.Method.Name;
                testButton.button.onClick.AddListener(action);
                return testButton;
            });
        }

        public override void InjectSceneViews(IObjectResolver resolver) => 
            resolver.Inject(popupsTesterSceneView);
    }
}