using UnityEngine;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Core.Popup.Scripts
{
    public interface IBasePopupFactory<out T> : IFactory<Transform, T> where T : BasePopup { }

    public class BasePopupFactory<T> : IBasePopupFactory<T> where T : BasePopup
    {
        private readonly IObjectResolver _resolver;
        private readonly T _basePopupFactoryPrefab;

        public BasePopupFactory(IObjectResolver resolver, T basePopupFactoryPrefab)
        {
            _resolver = resolver;
            _basePopupFactoryPrefab = basePopupFactoryPrefab;
        }

        public virtual T Create(Transform parentTransform)
        {
            var popupInstance = Object.Instantiate(_basePopupFactoryPrefab, parentTransform);
            _resolver.Inject(popupInstance);
            popupInstance.gameObject.SetActive(false);
            return popupInstance;
        }
    }
}