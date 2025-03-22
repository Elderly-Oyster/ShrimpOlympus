using System;
using System.Collections.Generic;
using MediatR;
using R3;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService
{
    
    public class LevelService
    {
        private readonly List<int> _levelsToUnlockNewFeature = new(){1, 3, 5, 15};
        private int _level;
        private float _experienceNeededForNextLevel;
        private int _numberOfUnlockedUpgrades;
        private int _experienceForProgressBar;
        private readonly Mediator _mediator;
        private readonly Subject<int> _levelSubject = new();
        private readonly Subject<float> _experienceSubject = new();
        private int _experience;

        private const float ExperienceMultiplier = 0.5F;
        private const int BaseExperienceForLevel = 100;
        
        public Observable<int> Level => _levelSubject.AsObservable();
        public Observable<float> ExperienceForProgressBar => _experienceSubject.AsObservable();

        public int Experience => _experience;


        public LevelService(Mediator mediator)
        {
            _mediator = mediator;
        }

        public void Initialize(int level, int experience, int numberOfFeatures)
        {
            _level = level;
            _experience = experience;
            _numberOfUnlockedUpgrades = numberOfFeatures;
            SetExperienceForLevel(_level);
        }

        public void GetExperience(int experience)
        {
            Debug.Log("GetExperience");
            _experience += experience;
            CalculateExperienceForProgressBar(_experience);
            CheckForLevelUp();
        }

        private void CalculateExperienceForProgressBar(int experience)
        {
           _experienceSubject.OnNext(experience/_experienceNeededForNextLevel);
        }

        private void SetExperienceForLevel(int level)
        {
            _experienceNeededForNextLevel += BaseExperienceForLevel * ExperienceMultiplier * (level+1);
        }

        private async void LevelUp()
        {
            _level++;
            _levelSubject.OnNext(_level);
            await _mediator.Send(new LevelServiceOperations.LevelUpCommand(_level));
            SetExperienceForLevel(_level);
            CheckToUnlockNewUpgrade(_level);
            _experience = 0;
            CalculateExperienceForProgressBar(_experience);
        }

        private void CheckForLevelUp()
        {
            if (_experience >= _experienceNeededForNextLevel)
                LevelUp();
        }

        private async void CheckToUnlockNewUpgrade(int level)
        {
            if (_levelsToUnlockNewFeature.Contains(level))
            {
                await _mediator.Send(new LevelServiceOperations.NewUpgradeUnlockedCommand(++_numberOfUnlockedUpgrades));
            }
        }
    }
}