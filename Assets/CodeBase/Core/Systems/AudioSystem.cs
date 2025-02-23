using System;
using Core.Scripts.Services.DataPersistenceSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Systems
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSystem : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioClip mainMelodyClip;
        [SerializeField] private AudioClip gameMelodyClip;
        [Header("Fade Parameters")]
        [SerializeField] private float fadeDuration = 1.0f;  
        public float MusicVolume { get; private set; }
        public float SoundsVolume { get; private set; }
        public event Action<float> OnSoundsVolumeChanged;

        public void PlayGameMelody() => PlayMusic(gameMelodyClip);
        public void PlayMainMenuMelody() => PlayMusic(mainMelodyClip);

        private void PlayMusic(AudioClip music)
        {
            musicAudioSource.clip = music;
            FadeIn(musicAudioSource, MusicVolume, fadeDuration);
        }
        public void StopMusic() => FadeOut(musicAudioSource, fadeDuration);

        public void SetMusicVolume(float volume)
        {
            Debug.Log("Set MusicVolume - " + volume);
            if (volume > 0)
            {
                MusicVolume = volume;
                musicAudioSource.volume = MusicVolume;
            }
            else
            {
                MusicVolume = 0;    
                musicAudioSource.volume = MusicVolume;
            }
        }

        public void SetSoundsVolume(float volume)
        {
            SoundsVolume = volume > 0 ? volume : 0;
            OnSoundsVolumeChanged?.Invoke(SoundsVolume);
        }

        private async void FadeOut(AudioSource audioSource, float duration)
        {
            var startVolume = audioSource.volume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                await UniTask.Yield();
            }

            audioSource.volume = 0; 
            audioSource.Stop();
        }

        private async void FadeIn(AudioSource audioSource, float targetVolume, float duration)
        {
            if (!audioSource.isPlaying) audioSource.Play();
            var startVolume = audioSource.volume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t / duration);
                await UniTask.Yield();
            }
    
            audioSource.volume = targetVolume;
        }

        public void SaveData(ref GameData gameData)
        {
            gameData.musicVolume = MusicVolume;
            gameData.soundsVolume = SoundsVolume;
        }

        public void LoadData(GameData gameData)
        {
            MusicVolume = gameData.musicVolume;
            SoundsVolume = gameData.soundsVolume;
            musicAudioSource.volume = MusicVolume;
            
            Debug.Log(MusicVolume + "Music");
            Debug.Log(SoundsVolume + "Sounds");
        }
    }
}