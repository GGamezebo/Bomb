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

        protected override void Start()
        {
            base.Start();
            textComponent = GetComponent<TMPro.TextMeshProUGUI>();
            _eventListener.Add(Events.EvCountDownTickChanged, new Action<int>(OnCountDownTickChanged));
        }

        void OnCountDownTickChanged(int count)
        {
            textComponent.text = $"{count:0}";
        }

        protected override void OnStateChanged(GameState state)
        {
            if (state == GameState.Play)
            {
                textComponent.text = "";
            }
        }
    }
}
