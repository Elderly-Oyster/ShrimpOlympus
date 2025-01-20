using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scripts.Views.ProgressBars
{
    public abstract class BaseProgressBarView : MonoBehaviour, IProgress<float>
    {
        public abstract void Report(float value);
        public abstract void ReportToZero(float value);

        public bool canAnimate;
        public bool canAnimateToZero;
        private float _currentRatio; //it is initialized to 0 by default 

        public async UniTask Animate(float duration, float value = 1f)
        {
            canAnimate = true;
            var ratio = _currentRatio;
            var multiplier = value / duration;
             while (ratio < value && canAnimate)
            {
                _currentRatio = ratio;
                ratio += Time.deltaTime * multiplier;
                Report(ratio);
                await UniTask.Yield();
            }
            canAnimate = false;
            _currentRatio = 0;
        }
        
        public async UniTask AnimateToZero(float duration, float currentValue)
        {
            canAnimateToZero = true;
            var ratio = currentValue;
            var multiplier = currentValue / duration;
            while (ratio > 0 && canAnimateToZero)
            {
                ratio -= Time.deltaTime * multiplier;
                ReportToZero(ratio);
                await UniTask.Yield();
            }
            canAnimateToZero = false;
        }

        public void PauseAnimate()
        {
            canAnimate = false;
        }

        public void ResumeAnimation()
        {
            canAnimate = true;
            Animate(_currentRatio).Forget();
        }
    }
}