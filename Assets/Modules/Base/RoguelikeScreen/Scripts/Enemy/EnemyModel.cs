using R3;
using UnityEngine;

namespace Modules.Base.RoguelikeScreen.Scripts.Enemy
{
    public class EnemyModel
    {
        //TODO: вместо const заебашить инициализатор через ScriptableObject
        public const float MaxHealth = 100f;
        public const float MoveSpeed = 2f;
        public const int Damage = 10;
        public const float ShootRadius = 3f;
        public const float ChaseRadius = 5f;
        public const float WanderRadiusMin = 2f;
        public const float WanderRadiusMax = 3f;

        public ReactiveProperty<Vector2> Position { get; } = new();
        public ReactiveProperty<float> Health { get; } = new(MaxHealth);
        
        public void Move(Vector2 directionUnnormalized) => Position.Value += 
            directionUnnormalized.normalized * (MoveSpeed * Time.deltaTime);
        
        public void TakeDamage(float damage) => Health.Value -= damage;
    }
}