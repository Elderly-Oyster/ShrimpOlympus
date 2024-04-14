using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.EnergyBar
{
    public class EnergyBarTimerView : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text text;

        private int _currentSeconds;
        private bool _stopTimer = true;
        private CancellationTokenSource _cancellationTokenSource;

        public Action TimerElapsed;

        private void Awake()
        {
            PlayerLoopTimer.StartNew(
                new TimeSpan(0,0, 1), true, 
                DelayType.Realtime, PlayerLoopTiming.LastFixedUpdate, CancellationToken.None, UpdateTimer, null);
        }
        
        public void RunTimer(int seconds)
        {
            if(!_stopTimer)
                return;
            gameObject.SetActive(true);
            _stopTimer = false;
            _currentSeconds = seconds;
            if (slider.IsActive())
            {
                slider.maxValue = seconds;
                slider.value = seconds;
            }
            text.text = $"{seconds / 60:0}:{seconds % 60:00}";
        }
        
        public void StopTimer()
        {
            gameObject.SetActive(false);
            _stopTimer = true;
        }
        
        public bool IsTimerRunning() => !_stopTimer;

        private async void UpdateTimer(object ob)
        {
            if(_stopTimer) 
                return;
            _currentSeconds--;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            await DOTween.To(() => slider.value, x => slider.value = x, _currentSeconds, 1f)
                .SetEase(Ease.Linear).ToUniTask(cancellationToken: _cancellationTokenSource.Token);
            slider.value = _currentSeconds;
            text.text = $"{_currentSeconds / 60:0}:{_currentSeconds % 60:00}";
            if (_currentSeconds <= 0)
            {
                _stopTimer = true;
                TimerElapsed?.Invoke();
            }
        }
    }
}