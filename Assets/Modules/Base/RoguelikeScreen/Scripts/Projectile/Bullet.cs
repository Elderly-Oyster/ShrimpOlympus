using UnityEngine;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.Projectile
{
    public class Bullet : MonoBehaviour
    {
        private string _targetTag;
        private Vector2 _normalizedDirection;
        
        [SerializeField] private float damage;
        [SerializeField] private float speed;
        [SerializeField] private float maxDistance;
        private float _traveledDistance;
        
        [Inject]
        private IObjectResolver _resolver;
        private BulletMemoryPool _memoryPool;

        private void Start()
        {
            _memoryPool = _resolver.Resolve<BulletMemoryPool>();
        }

        private void OnEnable()
        {
            _traveledDistance = 0;
        }

        public void Fire(Vector2 startPos, Vector2 targetPos, string targetTag)
        {
            transform.position = startPos;
            _normalizedDirection = (targetPos - startPos).normalized;
            _targetTag = targetTag;
            
        }

        private void Update()
        {
            transform.Translate(_normalizedDirection * (speed * Time.deltaTime));
            _traveledDistance += speed * Time.deltaTime;
            if (_traveledDistance > maxDistance)
            {
                _memoryPool.Despawn(this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            {
                if (other.gameObject.CompareTag(_targetTag))
                {
                    other.GetComponent<IHittable>().Hit(damage);
                    _memoryPool.Despawn(this);
                }
            }
        }
    }
}