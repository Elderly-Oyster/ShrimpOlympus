using CodeBase.Core.Patterns.ObjectCreation;
using UnityEngine;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.ObjectPool
{
    public class MonoBehaviourPool<TBeh> : ObjectPool<TBeh> where TBeh : MonoBehaviour
    {
        private Transform _poolParent;
        public MonoBehaviourPool(IObjectResolver resolver, IFactory<TBeh> factory, Transform poolParent, int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, initialCapacity, maxSize)
        {
            _poolParent = poolParent;
        }
        
        protected override void OnCreated(TBeh item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(_poolParent);
        }

        protected override void OnDestroyed(TBeh item)
        {
            Object.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TBeh item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TBeh item)
        {
            item.gameObject.SetActive(false);
        }
    }
    
    public class MonoBehaviourPool<TParam1, TBeh> : ObjectPool<TParam1, TBeh> where TBeh : MonoBehaviour
    {
        private Transform _poolParent;
        public MonoBehaviourPool(IObjectResolver resolver, IFactory<TBeh> factory, Transform poolParent, int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, initialCapacity, maxSize)
        {
            _poolParent = poolParent;
        }
        
        protected override void OnCreated(TBeh item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(_poolParent);
        }

        protected override void OnDestroyed(TBeh item)
        {
            Object.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TBeh item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TBeh item)
        {
            item.gameObject.SetActive(false);
        }
    }
    
    public class MonoBehaviourPool<TParam1, TParam2, TBeh> : ObjectPool<TParam1, TParam2, TBeh> where TBeh : MonoBehaviour
    {
        private Transform _poolParent;
        public MonoBehaviourPool(IObjectResolver resolver, IFactory<TBeh> factory, Transform poolParent, int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, initialCapacity, maxSize)
        {
            _poolParent = poolParent;
        }
        
        protected override void OnCreated(TBeh item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(_poolParent);
        }

        protected override void OnDestroyed(TBeh item)
        {
            Object.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TBeh item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TBeh item)
        {
            item.gameObject.SetActive(false);
        }
    }
    
    public class MonoBehaviourPool<TParam1, TParam2, TParam3, TBeh> : ObjectPool<TParam1, TParam2, TParam3, TBeh> where TBeh : MonoBehaviour
    {
        private Transform _poolParent;
        public MonoBehaviourPool(IObjectResolver resolver, IFactory<TBeh> factory, Transform poolParent, int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, 0, maxSize)
        {
            _poolParent = poolParent;
            Prewarm(initialCapacity);
        }
        
        protected override void OnCreated(TBeh item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(_poolParent);
        }

        protected override void OnDestroyed(TBeh item)
        {
            Object.Destroy(item.gameObject);
        }

        protected override void OnSpawned(TBeh item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnDespawned(TBeh item)
        {
            item.gameObject.SetActive(false);
        }
    }
}