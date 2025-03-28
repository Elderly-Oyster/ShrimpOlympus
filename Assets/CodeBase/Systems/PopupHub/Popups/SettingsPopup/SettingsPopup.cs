using System;
using System.Globalization;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace CodeBase.Systems.PopupHub.Popups.SettingsPopup
{
    public class SettingsPopup : BasePopup
    { 
        [Header("Volume Sliders")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider soundVolumeSlider;
        [SerializeField] private TMP_Text musicVolumeText;
        [SerializeField] private TMP_Text soundVolumeText;
        
        [Inject] private AudioSystem _audioSystem;

        private void Start()
        {
            SetInitialSettings();
            SetUpSlidersListeners();
        }

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
        
        
    }
}