using System.Threading;
// using Core.UI.RewardablePopup;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Services.App;
using Services.EnergyBar;
// using Services.Combinations;
// using Services.Events.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Core.UI.EnergyBar
{
    public class EnergyBarView : MonoBehaviour
    {
        [Inject] private EnergyBarService _energyBarService;
        [Inject] private IApplicationService _applicationService;
        
        [SerializeField] private EnergyBarTimerView timerView;
        [SerializeField] private Slider energySlider;
        [SerializeField] private TMP_Text text;
        //[SerializeField] private RewardAnimation rewardAnimation;
        //[SerializeField] private Image fillImage;
        //[SerializeField] private Image energyIconImage; 
        //[SerializeField] private Sprite fillSprite;
        //[SerializeField] private Sprite overfillSprite;
        //[SerializeField] private Color overfillColor;
        //[SerializeField] private Sprite energyIcon;
        //[SerializeField] private Sprite overfillEnergyIcon;
        //[SerializeField] private TMP_Text energyCost;
        //[SerializeField] private InfinityEnergyView infinityEnergyView;
        //[SerializeField] private Transform commonEnergyView;
        
        private int _sliderValue;
        private int _timerCircle = 30;
        private CancellationTokenSource _cancellationTokenSource;

        public void Start()
        {
            _timerCircle = _energyBarService.GetEnergyBarDefaultSettings().refreshmentCooldownSec;
            energySlider.maxValue = _energyBarService.GetEnergyBarDefaultSettings().maxRenewableEnergy;
            _sliderValue = _energyBarService.GetUserEnergy();
            _energyBarService.EnergyChanged += OnEnergyChanged;
            SetSliderValue(_sliderValue);
            timerView.TimerElapsed += OnTimerElapsed;
            _applicationService.ApplicationPause += ApplicationPause;
            _applicationService.ApplicationResume += ApplicationResume;
            if (_energyBarService.GetUserEnergy() < _energyBarService.GetEnergyBarDefaultSettings().maxRenewableEnergy)
            {
                var remainedTime = _timerCircle - _energyBarService.GetTotalSecondsSinceLastUpdate() % _timerCircle;
                timerView.RunTimer(remainedTime);  
            }
            else
                timerView.StopTimer();
        }
        
        private void ApplicationResume()
        {
            if (_energyBarService.GetUserEnergy() < _energyBarService.GetEnergyBarDefaultSettings().maxRenewableEnergy)
                timerView.RunTimer(_timerCircle);
        }
        private void ApplicationPause()
        {
            timerView.StopTimer();
        }
        
        private void OnEnergyChanged(int oldEnergy, int newEnergy)
        {
            var oldSliderValue = _sliderValue;
            _sliderValue = newEnergy;
            SetSliderValue(_sliderValue);
            if (oldSliderValue >= _sliderValue && timerView.IsTimerRunning()) return;
            if (_sliderValue < _energyBarService.GetEnergyBarDefaultSettings().maxRenewableEnergy)
                timerView.RunTimer(_timerCircle);
            else
                timerView.StopTimer();
        }

        private void OnTimerElapsed() => 
            _energyBarService.AddSafeUserEnergy(_energyBarService.GetEnergyBarDefaultSettings().refreshmentEnergy);

        private async void SetSliderValue(int value)
        {
            energySlider.value = value;
            if (value > _energyBarService.GetEnergyBarDefaultSettings().maxRenewableEnergy)
            {
                // fillImage.sprite = overfillSprite;
                // fillImage.color = overfillColor;
                //energyIconImage.sprite = overfillEnergyIcon; 
            }
            else
            {
                // fillImage.sprite = fillSprite;
                // fillImage.color = Color.white;
                //energyIconImage.sprite = energyIcon;
            }

            await DOTween.To(() => float.Parse(text.text), x => text.text = ((int)x).ToString(), value, 1f).SetEase(Ease.Linear).ToUniTask();
        }

        private void Update()
        {
            //var energySettings = _energyBarService.GetEnergyBarDefaultSettings();
            // if (energySettings.InfiniteEnergyForever)
            // {
            //     if (infinityEnergyView.IsInfinity) return;
            //     infinityEnergyView.SetInfinity();
            //     infinityEnergyView.gameObject.SetActive(true);
            //     commonEnergyView.gameObject.SetActive(false);
            // } 
            // else if (energySettings.InfinityEnergyExpirationDate > DateTime.UtcNow)
            // {
            //     infinityEnergyView.gameObject.SetActive(true);
            //     var difference = energySettings.InfinityEnergyExpirationDate - DateTime.UtcNow;
            //     infinityEnergyView.amountText.text = difference.ToString(@"hh\:mm\:ss");
            //     commonEnergyView.gameObject.SetActive(false);
            // }
            // else
            // {
            //     // infinityEnergyView.gameObject.SetActive(false);
            //     commonEnergyView.gameObject.SetActive(true);
            // }
        }
        
        private void OnDestroy() 
        {
            timerView.StopTimer();
            _energyBarService.EnergyChanged -= OnEnergyChanged;
            timerView.TimerElapsed -= OnTimerElapsed;
            _applicationService.ApplicationPause -= ApplicationPause;
            _applicationService.ApplicationResume -= ApplicationResume;
        }
    }
}