using Modules.Base.RoguelikeScreen.Scripts.ObjectPool;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.Projectile
{
    public class BulletFactory : ResolverFactory<Bullet>
    {
        public BulletFactory(IObjectResolver resolver, Bullet prefab) : base(resolver, prefab)
        {}
    }
}