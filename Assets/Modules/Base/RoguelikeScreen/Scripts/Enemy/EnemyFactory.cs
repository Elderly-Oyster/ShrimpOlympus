using CodeBase.Core.Patterns.ObjectCreation;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.Enemy
{
    public class EnemyFactory : IFactory<EnemyView>
    {
        private IObjectResolver _resolver;

        public EnemyFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public EnemyView Create()
        {
            return _resolver.Resolve<EnemyView>();
        }
    }
}
