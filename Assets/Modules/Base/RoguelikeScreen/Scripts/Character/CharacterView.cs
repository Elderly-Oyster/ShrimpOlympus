using System;
using CodeBase.Services;
using Modules.Base.RoguelikeScreen.Scripts.Projectile;
using UnityEngine;
using VContainer;

namespace Modules.Base.RoguelikeScreen.Scripts.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterView : MonoBehaviour, IHittable
    {
        public event Action<Vector2> MoveInput;
        public event Action<Vector2> ShootTriggered;
        public event Action<float> DamageReceived;
        
        [Inject] private InputSystemService _input;

        private void Start()
        {
            _input.InputActions.RoguelikeCharacter.Shoot.performed += _ =>
                ShootTriggered?.Invoke(_input.InputActions.RoguelikeCharacter.Look
                    .ReadValue<Vector2>());
        }

        public void Hit(float damage) =>
            DamageReceived?.Invoke(damage);

        private void Update()
        {
            Vector2 moveInput = _input.InputActions.RoguelikeCharacter.Move.ReadValue<Vector2>();
            MoveInput?.Invoke(moveInput);
        }

        public void UpdatePosition(Vector2 position) => transform.position = position;
        private void OnEnable() => _input?.InputActions.Enable();
        private void OnDisable() => _input.InputActions.Disable();
    }
}