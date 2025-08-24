using System;
using MediatR;
using Modules.Base.RoguelikeScreen.Scripts.MediatorHandle;
using Modules.Base.RoguelikeScreen.Scripts.Projectile;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Modules.Base.RoguelikeScreen.Scripts.Character
{
    public class CharacterPresenter : IStartable, IDisposable
    {
        private readonly CharacterModel _model;
        private readonly CharacterView _view;
        private Camera _moduleCamera;
        private BulletMemoryPool _bulletMemoryPool;
        private Mediator _mediator;

        public CharacterPresenter(
            CharacterModel model,
            CharacterView view,
            Camera moduleCamera,
            BulletMemoryPool bulletMemoryPool,
            Mediator mediator)
        {
            _model = model;
            _view = view;
            _moduleCamera = moduleCamera;
            _bulletMemoryPool = bulletMemoryPool;
            _mediator = mediator;
        }

        public void Start()
        {
            _view.MoveInput += OnMoveInput;
            _view.ShootTriggered += OnShoot;
            _view.DamageReceived += OnDamage;

            Vector2 position = _view.transform.position;
            _model.Position.Value = position;

            _model.Position.Subscribe(_view.UpdatePosition);
            _model.Health.Where(hp => hp <= 0)
                .Subscribe(_ =>_mediator.Publish(new PlayerDeathNotification()));
        }

        private void OnMoveInput(Vector2 direction) => _model.Move(direction);

        private void OnShoot(Vector2 screenPosition)
        {
            var screenPositionWithDepth = new Vector3(screenPosition.x, screenPosition.y,
                -1f * _moduleCamera.transform.position.z);
            Vector3 worldPosition = _moduleCamera.ScreenToWorldPoint(screenPositionWithDepth);
            _bulletMemoryPool.Spawn(_view.transform.position, worldPosition, BulletType.Player);
        }

        private void OnDamage(float damage)
        {
            _model.TakeDamage(damage);
        }

        public void Dispose()
        {
            _view.MoveInput -= OnMoveInput;
            _view.ShootTriggered -= OnShoot;
            _view.DamageReceived -= OnDamage;
        }

        public void SetEnabled(bool enabled) => _view.gameObject.SetActive(enabled);

        public void Reset()
        {
            _model.Position.Value = Vector2.zero;
            _model.Health.Value = CharacterModel.MaxHealth;
        }
    }
}