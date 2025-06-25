using System;
using Common;
using GameLogic;
using UnityEngine;


namespace UI.HUD
{
    public class Countdown : GameObserverMonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI textComponent;

        protected virtual void OnEnable()
        {
            base.OnEnable();
            var i = 1;
        }

        protected override void Subscribe()
        {
            _eventListener.Add(Events.EvCountDownTickChanged, new Action<int>(OnCountDownTickChanged));
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnGameStateChanged));
        }

        void OnCountDownTickChanged(int count)
        {
            textComponent.text = $"{count:0}";
        }

        void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Play)
            {
                textComponent.text = "";
            }
        }
    }
}
