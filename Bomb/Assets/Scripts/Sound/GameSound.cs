using System;
using Common;
using GameLogic;
using JetBrains.Annotations;
using UnityEngine;

namespace Sound
{

    public static class SoundUtils
    {
        public static void PlayLoop([CanBeNull] AudioSource audioSource, [CanBeNull] AudioClip audioClip)
        {
            if (audioSource && audioClip)
            {
                audioSource.clip = audioClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        
        public static void PlayOneShot([CanBeNull] AudioSource audioSource, [CanBeNull] AudioClip audioClip)
        {
            if (audioSource&& audioClip)
            {
                audioSource.PlayOneShot(audioClip);
            }
        }
    }

    public class GameSound : GameObserverMonoBehaviour
    {

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip countdownTickShot;

        [SerializeField]
        private AudioClip playShot;
        
        [SerializeField]
        private AudioClip explosionClip;        
        
        [SerializeField]
        private AudioSource tickAudioSource;
        
        [SerializeField]
        private AudioClip playTickLoop;        
      
        [SerializeField]
        private AudioClip alertTickLoop;      
        
        [SerializeField]
        private AudioSource musicAudioSource;
        
        [SerializeField]
        private AudioClip playMusicLoop;
        
        protected override void Start()
        {
            base.Start();
            _eventListener.Add(Events.EvCountDownTickChanged, new Action<int>(OnCountDownTickChanged));
        }

        void OnCountDownTickChanged(int count)
        {
            SoundUtils.PlayOneShot(audioSource, countdownTickShot);
        }  
        
        protected override void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Play:
                    SoundUtils.PlayOneShot(audioSource, playShot);
                    SoundUtils.PlayLoop(tickAudioSource, playTickLoop);
                    SoundUtils.PlayLoop(musicAudioSource, playMusicLoop);
                    break;
                case GameState.Explosion:
                    SoundUtils.PlayOneShot(audioSource, explosionClip);
                    tickAudioSource.Stop();
                    musicAudioSource.Stop();
                    break;
            }
        }
        
        protected override void OnAlert()
        {
            SoundUtils.PlayLoop(tickAudioSource, alertTickLoop);
        }
        
    }
}
