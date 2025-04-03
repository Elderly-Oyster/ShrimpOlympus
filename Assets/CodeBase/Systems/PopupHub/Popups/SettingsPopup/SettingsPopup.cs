using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub;
using CodeBase.Core.Systems.PopupHub.Popups;
using CodeBase.Systems.InputSystem;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace CodeBase.Systems.PopupHub.Popups.SettingsPopup
{
    public class SettingsPopup : BasePopup, IEscapeListener
    { 
        [Header("Volume Sliders")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider soundVolumeSlider;
        [SerializeField] private TMP_Text musicVolumeText;
        [SerializeField] private TMP_Text soundVolumeText;
        [SerializeField] private Button rebindPopupButton;
        
        [Inject] private AudioSystem _audioSystem;
        [Inject] private InputSystem.InputSystem _inputSystem;
        [Inject] private IObjectResolver _objectResolver;
        
        private IPopupHub _popupHub;
        
        private readonly ReactiveCommand<Unit> _rebindPopupCommand = new();

        private void Start()
        {
            _inputSystem.AddEscapeListener(this);
            SetInitialSettings();
            SetUpSlidersListeners();
            _rebindPopupCommand.Subscribe(_ => OnOpenRebindPopupButtonClicked());
            rebindPopupButton.OnClickAsObservable()
                .Subscribe(_ => _rebindPopupCommand.Execute(default))
                .AddTo(this);
        }

        public async void OnEscapePressed() => await Close();

        private void SetUpSlidersListeners()
        {
            musicVolumeSlider.onValueChanged.AddListener(v => _audioSystem.SetMusicVolume(v));
            musicVolumeSlider.onValueChanged.
                AddListener(v => musicVolumeText.text = ((int)(v * 100)).ToString());
            soundVolumeSlider.onValueChanged.AddListener(v => _audioSystem.SetSoundsVolume(v));
            soundVolumeSlider.onValueChanged.
                AddListener(v => soundVolumeText.text = ((int)(v * 100)).ToString());
        }

        private void SetInitialSettings()
        {
            musicVolumeSlider.value = _audioSystem.MusicVolume * 100;
            musicVolumeText.text = ((int)(_audioSystem.MusicVolume * 100)).ToString();
            soundVolumeSlider.value = _audioSystem.SoundsVolume * 100;
            soundVolumeText.text = ((int)(_audioSystem.SoundsVolume * 100)).ToString();
        }

        private void OnOpenRebindPopupButtonClicked()
        {
            if (_popupHub == null)
                _popupHub = _objectResolver.Resolve<IPopupHub>();
        }
    }
}