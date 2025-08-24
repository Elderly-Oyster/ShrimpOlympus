using CodeBase.Core.Patterns.ObjectCreation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.RoguelikeScreen.Scripts.ObjectPool
{
    public class ResolverFactory<TPrefab> : IFactory<TPrefab> where TPrefab : MonoBehaviour
    {
        private IObjectResolver _resolver;
        private TPrefab _prefab;

        public ResolverFactory(IObjectResolver resolver, TPrefab prefab)
        {
            _resolver = resolver;
            _prefab = prefab;
        }
        
        public TPrefab Create()
        {
            return _resolver.Instantiate(_prefab);
        }
    }
}