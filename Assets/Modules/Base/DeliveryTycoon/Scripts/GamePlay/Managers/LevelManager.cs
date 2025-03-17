using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    
    public class LevelManager
    {
        private List<int> _levelsToUnlockNewFeature = new(){1, 3, 5, 15};
        
        private ReactiveProperty<int> _level = new ();
        private ReactiveProperty<int> _experience = new ();
        private const float EXPERIENCE_MULTIPLIER = 0.5F;
        private const int BaseExperienceForLevel = 100;
        private float _experienceNeededForNextLevel;
        private ReactiveProperty<int> _numberOfUnlockedFeatures = new ();
        
        public event Action<float> OnUpdateViewProgressBar;
        
        public ReadOnlyReactiveProperty<int> Level => _level;

        public ReadOnlyReactiveProperty<int> Experience => _experience;

        public ReadOnlyReactiveProperty<int> NumberOfUnlockedFeatures => _numberOfUnlockedFeatures;

        public void Initialize(int level, int experience, int numberOfFeatures)
        {
            _level.Value = level;
            _experience.Value = experience;
            _numberOfUnlockedFeatures.Value = numberOfFeatures;
            SetExperienceForLevel(_level.Value);
        }

        public void GetExperience(int experience)
        {
            Debug.Log("GetExperience");
            _experience.Value += experience;
            OnUpdateViewProgressBar?.Invoke(CalculateExperienceForProgressBar(_experience.Value));
            CheckForLevelUp();
        }

        private float CalculateExperienceForProgressBar(int experience)
        {
            return experience/_experienceNeededForNextLevel;
        }

        private void SetExperienceForLevel(int level)
        {
            _experienceNeededForNextLevel += BaseExperienceForLevel * EXPERIENCE_MULTIPLIER * (level+1);
        }

        private void LevelUp()
        {
            _level.Value++;
            SetExperienceForLevel(_level.Value);
            CheckToUnlockNewFeature(_level.Value);
            _experience.Value = 0;
            OnUpdateViewProgressBar?.Invoke(CalculateExperienceForProgressBar(_experience.Value));
        }

        private void CheckForLevelUp()
        {
            if (_experience.Value >= _experienceNeededForNextLevel)
                LevelUp();
        }

        private void CheckToUnlockNewFeature(int level)
        {
            if (_levelsToUnlockNewFeature.Contains(level))
            {
                _numberOfUnlockedFeatures.Value++;
            }
        }
    }
}