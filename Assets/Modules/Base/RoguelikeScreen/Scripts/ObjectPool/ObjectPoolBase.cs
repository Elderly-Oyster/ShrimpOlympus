using System.Collections.Generic;
using System.Linq;
using CodeBase.Core.Patterns.ObjectCreation;
using UnityEngine;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.ObjectPool
{
    public abstract class ObjectPoolBase<TValue> : IObjectPool
    {
        private IObjectResolver _resolver;
        private IFactory<TValue> _factory;
        private Stack<TValue> _inactiveItems;
        private HashSet<TValue> _activeItems;

        private int _maxSize;

        public ObjectPoolBase(IObjectResolver resolver, IFactory<TValue> factory,
            int initialCapacity = 0, int maxSize = -1)
        {
            _resolver = resolver;
            _factory = factory;
            _inactiveItems = new Stack<TValue>(initialCapacity);
            _activeItems = new HashSet<TValue>(initialCapacity);

            _maxSize = Mathf.Clamp(maxSize, -1, int.MaxValue);

            Prewarm(initialCapacity);
        }

        public int ActiveCount => _activeItems.Count;
        public int InactiveCount => _inactiveItems.Count;
        public int TotalCount => ActiveCount + InactiveCount;

        public void Prewarm(int count) => Resize(count);

        public void Clear() => Resize(0);

        public void Resize(int desiredSize)
        {
            if (desiredSize < 0)
                desiredSize = 0;
            if (desiredSize > _maxSize && _maxSize != -1)
                desiredSize = _maxSize;

            while (_inactiveItems.Count > desiredSize)
            {
                OnDestroyed(_inactiveItems.Pop());
            }

            while (desiredSize > _inactiveItems.Count)
            {
                _inactiveItems.Push(InstantiateNew());
            }
        }

        public void DespawnAll()
        {
            foreach (var item in _activeItems.ToList())
            {
                Despawn(item);
            }
        }

        public void Despawn(TValue item)
        {
            if (!_activeItems.Contains(item) &&  !_inactiveItems.Contains(item))
                throw new System.ArgumentException($"Object {item} does not exist in pool {this}");
            _activeItems.Remove(item);
            _inactiveItems.Push(item);

            OnDespawned(item);

            if (_inactiveItems.Count > _maxSize && _maxSize != -1)
            {
                Resize(_maxSize);
            }
        }

        protected TValue SpawnInternal()
        {
            if (_inactiveItems.Count == 0)
            {
                ExpandPool();
            }

            var item = _inactiveItems.Pop();
            _activeItems.Add(item);
            OnSpawned(item);
            return item;
        }

        private TValue InstantiateNew()
        {
            var instance = _factory.Create();
            OnCreated(instance);
            return instance;
        }

        private void ExpandPool()
        {
            if (TotalCount == 0)
            {
                Resize(1);
            }
            else
            {
                Resize(TotalCount * 2);
            }
        }


        protected virtual void OnDespawned(TValue item)
        {
        }

        protected virtual void OnSpawned(TValue item)
        {
        }

        protected virtual void OnCreated(TValue item)
        {
        }

        protected virtual void OnDestroyed(TValue item)
        {
        }
    }
}