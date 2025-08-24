using System;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Services;
using Modules.Base.RoguelikeScreen.Scripts.Character;
using Modules.Base.RoguelikeScreen.Scripts.Enemy;
using Modules.Base.RoguelikeScreen.Scripts.Projectile;
using Modules.Base.RoguelikeScreen.Scripts.Service;
using R3;
using Stateless;

namespace Modules.Base.RoguelikeScreen.Scripts
{
    public enum ScreenState
    {
        Gameplay,
        GameOver
    }

    public enum ScreenChangeTrigger
    {
        ToGameplay,
        ToGameOver
    }

    public class RoguelikeScreenModel : IModel
    {
        private readonly StateMachine<ScreenState, ScreenChangeTrigger> _stateMachine;
        public readonly ReactiveProperty<int> Score = new(0);

        private readonly CharacterPresenter _characterPresenter;
        private readonly BulletMemoryPool _bulletMemoryPool;
        private readonly EnemyMemoryPool _enemyMemoryPool;
        private readonly EnemySpawner _enemySpawner;
        private readonly InputSystemService _inputSystemService;

        public event Action OnGameplayEnterEvent;
        public event Action OnGameOverEnterEvent;

        public RoguelikeScreenModel(CharacterPresenter characterPresenter, BulletMemoryPool bulletMemoryPool,
            EnemyMemoryPool enemyMemoryPool, EnemySpawner enemySpawner, InputSystemService inputSystemService)
        {
            _characterPresenter = characterPresenter;
            _bulletMemoryPool = bulletMemoryPool;
            _enemyMemoryPool = enemyMemoryPool;
            _enemySpawner = enemySpawner;
            _inputSystemService = inputSystemService;
            
            
            _stateMachine = new StateMachine<ScreenState, ScreenChangeTrigger>(ScreenState.Gameplay);
            _stateMachine.Configure(ScreenState.Gameplay)
                .OnEntry(OnGameplayEnter)
                .Permit(ScreenChangeTrigger.ToGameOver, ScreenState.GameOver);
            _stateMachine.Configure(ScreenState.GameOver)
                .OnEntry(OnGameOverEnter)
                .Permit(ScreenChangeTrigger.ToGameplay, ScreenState.Gameplay);
            OnGameplayEnter();
        }

        public void TriggerStateChange(ScreenChangeTrigger trigger) => _stateMachine.Fire(trigger);

        private void OnGameplayEnter()
        {
            OnGameplayEnterEvent?.Invoke();
            
            _characterPresenter.Reset();
            _characterPresenter.SetEnabled(true);
            Score.Value = 0;
            _enemySpawner.Start();
            _inputSystemService.SwitchToRoguelikeCharacter();
        }

        private void OnGameOverEnter()
        {
            OnGameOverEnterEvent?.Invoke();
            
            _characterPresenter.SetEnabled(false);
            _inputSystemService.SwitchToUI();
            _enemyMemoryPool.DespawnAll();
            _bulletMemoryPool.DespawnAll();
            _enemySpawner.Stop();
        }

        public void Dispose()
        {
        }
    }
}