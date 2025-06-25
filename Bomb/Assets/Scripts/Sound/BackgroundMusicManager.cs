using Common;
using UnityEngine;

namespace Sound
{

    public class BackgroundMusicManager : GameObserverMonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip backgroundMusic;

        private void Start()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Дополнительные методы для управления музыкой:
        public void StopMusic()
        {
            audioSource.Stop();
        }

        public void PauseMusic()
        {
            audioSource.Pause();
        }

        public void ResumeMusic()
        {
            audioSource.UnPause();
        }

        public void SetVolume(float volume)
        {
            audioSource.volume = volume;
        }
    }
}