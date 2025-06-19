using System;
using Common;
using GameLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Countdown : GameObserverMonoBehaviour
    {
        TMPro.TextMeshProUGUI textComponent;

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

        protected override void Start()
        {
            base.Start();
            textComponent = GetComponent<TMPro.TextMeshProUGUI>();
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
