using CodeBase.Core.Patterns.ObjectCreation;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.ObjectPool
{
    public class ObjectPool<TValue> : ObjectPoolBase<TValue>, IObjectPool<TValue>
    {
        public ObjectPool(IObjectResolver resolver, IFactory<TValue> factory, int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, initialCapacity, maxSize)
        {
        }

        public TValue Spawn()
        {
            var item = SpawnInternal();
            Reinitialize(item);
            return item;
        }
        
        protected virtual void Reinitialize(TValue item)
        {
        }
    }

    public class ObjectPool<TParam1, TValue> : ObjectPoolBase<TValue>, IObjectPool<TParam1, TValue>
    {
        public ObjectPool(IObjectResolver resolver, IFactory<TValue> factory, int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, initialCapacity, maxSize)
        {
        }

        public TValue Spawn(TParam1 param1)
        {
            var item = SpawnInternal();
            Reinitialize(param1, item);
            return item;
        }
        
        protected virtual void Reinitialize(TParam1 param1, TValue item)
        {
        }
    }

    public class ObjectPool<TParam1, TParam2, TValue> : ObjectPoolBase<TValue>, IObjectPool<TParam1, TParam2, TValue>
    {
        public ObjectPool(IObjectResolver resolver, IFactory<TValue> factory, int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, initialCapacity, maxSize)
        {
        }

        public TValue Spawn(TParam1 param1, TParam2 param2)
        {
            var item = SpawnInternal();
            Reinitialize(param1, param2, item);
            return item;
        }
        
        protected virtual void Reinitialize(TParam1 param1, TParam2 param2, TValue item)
        {
        }
    }
    public class ObjectPool<TParam1, TParam2, TParam3, TValue> : ObjectPoolBase<TValue>, IObjectPool<TParam1, TParam2, TParam3, TValue>
         {
             public ObjectPool(IObjectResolver resolver, IFactory<TValue> factory, int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, initialCapacity, maxSize)
             {
             }
     
             public TValue Spawn(TParam1 param1, TParam2 param2, TParam3 param3)
             {
                 var item = SpawnInternal();
                 Reinitialize(param1, param2, param3, item);
                 return item;
             }
             
             protected virtual void Reinitialize(TParam1 param1, TParam2 param2, TParam3 param3, TValue item)
             {
             }
         }
}