using System;
using Common;
using UnityEngine;


namespace UI.HUD
{
    public class Countdown : GameObserverMonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI textComponent;

        protected override void Subscribe()
        {
            EventListener.Add(Events.EvCountDownTickChanged, new Action<int>(OnCountDownTickChanged));
        }

        void OnCountDownTickChanged(int count)
        {
            textComponent.text = $"{count:0}";
        }
    }
}
