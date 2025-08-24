using System;
using Modules.Base.RoguelikeScreen.Scripts.Projectile;
using UnityEngine;

namespace Modules.Base.RoguelikeScreen.Scripts.Enemy
{
    public class EnemyView : MonoBehaviour, IHittable
    {
        [SerializeField] private Rigidbody2D rigidbody2D;
        [SerializeField] private Transform gunPoint;

        public event Action Tick;
        public event Action<float> DamageReceived;
        public event Action ResetEvent;

        private void Update()
        {
            Tick?.Invoke();
        }

        public void Reset()
        {
            ResetEvent?.Invoke();
        }

        public void Hit(float damage) => DamageReceived?.Invoke(damage);

        public Vector2 GunPosition => gunPoint.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, EnemyModel.ShootRadius);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, EnemyModel.ChaseRadius);

        }
    }
}