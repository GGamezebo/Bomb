using Common;
using GameLogic;
using System;
using UnityEngine;

namespace Lib.Unity.Haptic
{
    public class HapticsManager : MonoBehaviour
    {
        private EventListener _eventListener;

        private void Start()
        {
            var globalContext = FindFirstObjectByType<GlobalContext>();
            _eventListener = globalContext.MakeEventListener();
            Subscribe();
        }

        private void OnDisable()
        {
            _eventListener.RemoveAllListeners();
        }

        private void Subscribe()
        {
            _eventListener.Add(Events.EvAlert, new Action(OnAlert));
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnGameStateChanged));
        }

        private void OnAlert()
        {
            HapticFeedback.Generate(HapticFeedback.HapticTypes.Alert);
        }

        private void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Play:
                    HapticFeedback.Generate(HapticFeedback.HapticTypes.Play);
                    break;
                case GameState.Explosion:
                    HapticFeedback.Generate(HapticFeedback.HapticTypes.Explosion);
                    break;
            }
        }

        public void ToggleHaptics(bool enabled)
        {
            HapticFeedback.HapticsEnabled = enabled;
        }
    }
}