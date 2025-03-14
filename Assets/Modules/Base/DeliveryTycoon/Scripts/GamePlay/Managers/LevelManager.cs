using System;
using System.Collections.Generic;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    
    public class LevelManager
    {
        private List<int> _levelsToUnlockNewFeature = new(){1, 3, 5, 15};
        
        private int _level;
        private int _experience;
        private const float EXPERIENCE_MULTIPLIER = 0.5F;
        private const int BaseExperienceForLevel = 100;
        private float _experienceNeededForNextLevel;
        private int _numberOfUnlockedFeatures;

        public event Action<int> OnLevelUp;
        public event Action<float> OnUpdateViewProgressBar;

        public int Level => _level;

        public int Experience => _experience;

        public int NumberOfUnlockedFeatures => _numberOfUnlockedFeatures;

        public void Initialize(int level, int experience, int numberOfFeatures)
        {
            _level = level;
            _experience = experience;
            _numberOfUnlockedFeatures = numberOfFeatures;
            SetExperienceForLevel(_level);
        }

        public void GetExperience(int experience)
        {
            _experience += experience;
            OnUpdateViewProgressBar?.Invoke(CalculateExperienceForProgressBar(_experience));
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
            _level++;
            SetExperienceForLevel(_level);
            CheckToUnlockNewFeature(_level);
            _experience = 0;
            OnUpdateViewProgressBar?.Invoke(CalculateExperienceForProgressBar(_experience));
            OnLevelUp?.Invoke(_level);
        }

        private void CheckForLevelUp()
        {
            if (_experience >= _experienceNeededForNextLevel)
                LevelUp();
        }

        private void CheckToUnlockNewFeature(int level)
        {
            if (_levelsToUnlockNewFeature.Contains(level))
            {
                _numberOfUnlockedFeatures++;
            }
        }
    }
}