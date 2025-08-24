using CodeBase.Core.Patterns.ObjectCreation;
using Modules.Base.RoguelikeScreen.Scripts.ObjectPool;
using UnityEngine;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.Enemy
{
    public class EnemyMemoryPool : MonoBehaviourPool<Vector2, EnemyView>
    {
        public EnemyMemoryPool(IObjectResolver resolver, IFactory<EnemyView> factory, Transform poolParent, int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, poolParent, 0, maxSize)
        {
        }

        protected override void Reinitialize(Vector2 param1, EnemyView item)
        {
            item.transform.position = param1;
            item.Reset();
        }
    }
}