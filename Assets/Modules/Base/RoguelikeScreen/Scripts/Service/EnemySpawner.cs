using System;
using Modules.Base.RoguelikeScreen.Scripts.Character;
using Modules.Base.RoguelikeScreen.Scripts.Enemy;
using R3;
using UnityEngine;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace Modules.Base.RoguelikeScreen.Scripts.Service
{
    public class EnemySpawner
    {
        private float _spawnCooldown;
        private float _spawnRange;
        private EnemyMemoryPool _enemyMemoryPool;
        private CharacterModel _characterModel;
        private Func<float, int> _batchSizeFunc;

        private float _passedTime;

        private IDisposable _timerDisposable;

        public EnemySpawner(float spawnCooldown, float spawnRange, EnemyMemoryPool enemyMemoryPool,
            CharacterModel characterModel, Func<float, int> batchSizeFunc)
        {
            _spawnCooldown = spawnCooldown;
            _spawnRange = spawnRange;
            _enemyMemoryPool = enemyMemoryPool;
            _characterModel = characterModel;
            _batchSizeFunc = batchSizeFunc;
        }

        public void Start()
        {
            _timerDisposable =
                Observable.Interval(TimeSpan.FromSeconds(_spawnCooldown)).Subscribe(_ => Spawn());
        }

        public void Stop()
        {
            _timerDisposable?.Dispose();
        }

        private void Spawn()
        {
            Vector2 center = _characterModel.Position.Value;
            _passedTime += _spawnCooldown;
            var batchSize = _batchSizeFunc(_passedTime);
            while (batchSize > 0)
            {
                batchSize--;
                Vector2 position = center + Random.insideUnitCircle.normalized * _spawnRange;
                _enemyMemoryPool.Spawn(position);
            }
        }
    }
}