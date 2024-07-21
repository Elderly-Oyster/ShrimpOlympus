using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Core.Popup
{
    public interface IBasePopupFactory<out T> : IFactory<Transform, T> where T : BasePopup { }

    public class BasePopupFactory<T> : IBasePopupFactory<T> where T : BasePopup
    {
        private readonly IObjectResolver _resolver;
        private readonly BasePopup _basePopupFactoryPrefab;

        public BasePopupFactory(IObjectResolver resolver, BasePopup basePopupFactoryPrefab)
        {
            _resolver = resolver;
            _basePopupFactoryPrefab = basePopupFactoryPrefab;
        }

        public virtual T Create(Transform transform)
        {
            var popupInstance = Object.Instantiate(_basePopupFactoryPrefab, transform);
            _resolver.Inject(popupInstance);
            popupInstance.gameObject.SetActive(false);
            return popupInstance as T;
        }
    }
}