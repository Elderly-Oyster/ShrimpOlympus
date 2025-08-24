using R3;
using UnityEngine;

namespace Modules.Base.RoguelikeScreen.Scripts.Character
{
    public class CharacterModel
    {
        public ReactiveProperty<Vector2> Position { get; } = new();
        public ReactiveProperty<float> Health { get; } = new(100);
    
        public const float MaxHealth = 100f;
        public const float MoveSpeed = 5f;
        public const int Damage = 25;
    
        public void Move(Vector2 direction) => Position.Value += direction * (MoveSpeed * Time.deltaTime);
        public void TakeDamage(float damage) => Health.Value -= damage;
    }
}