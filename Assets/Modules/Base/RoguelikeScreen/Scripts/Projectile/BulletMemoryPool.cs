using CodeBase.Core.Patterns.ObjectCreation;
using Modules.Base.RoguelikeScreen.Scripts.ObjectPool;
using UnityEngine;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.Projectile
{
    public enum BulletType
    {
        Player,
        Enemy
    }

    public class BulletMemoryPool : MonoBehaviourPool<Vector2, Vector2, BulletType, Bullet>
    {
        public BulletMemoryPool(IObjectResolver resolver, IFactory<Bullet> factory, Transform poolParent,
            int initialCapacity = 0, int maxSize = -1) : base(resolver, factory, poolParent, initialCapacity, maxSize)
        {
        }

        protected override void Reinitialize(Vector2 param1, Vector2 param2, BulletType param3, Bullet item)
        {
            var bulletColor = (param3 == BulletType.Player) ? Color.white : Color.red;
            item.GetComponent<SpriteRenderer>().color = bulletColor;
            var targetTag = (param3 == BulletType.Player) ? "Enemy" : "Player";
            item.Fire(param1, param2, targetTag);
        }
    }
}