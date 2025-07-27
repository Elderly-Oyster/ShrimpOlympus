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
        private readonly IMediator _mediator;
        private readonly Subject<int> _levelSubject = new();
        private readonly Subject<float> _experienceSubject = new();

        private const float ExperienceMultiplier = 0.5F;
        private const int BaseExperienceForLevel = 100;
        
        public Observable<int> Level => _levelSubject.AsObservable();
        public Observable<float> ExperienceForProgressBar => _experienceSubject.AsObservable();

        public int Experience { get; private set; }


        public LevelService(IMediator mediator) => _mediator = mediator;

        public void Initialize(int level, int experience, int numberOfFeatures)
        {
            _level = level;
            Experience = experience;
            _numberOfUnlockedUpgrades = numberOfFeatures;
            SetExperienceForLevel(_level);
        }

        public void GetExperience(int experience)
        {
            Debug.Log("GetExperience");
            Experience += experience;
            CalculateExperienceForProgressBar(Experience);
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
            Experience = 0;
            CalculateExperienceForProgressBar(Experience);
        }

        private void CheckForLevelUp()
        {
            if (Experience >= _experienceNeededForNextLevel)
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