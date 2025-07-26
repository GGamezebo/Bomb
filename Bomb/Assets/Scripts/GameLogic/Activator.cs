using System;
using System.Collections.Generic;
using UnityEngine;


namespace Common
{
    public sealed class Activator : GameObserverMonoBehaviour
    {
        [SerializeField] private List<GameLogic.GameState> states;
        [SerializeField] private GameObject target;
        
        protected override void Subscribe()
        {
            EventListener.Add(Events.EvGameStateChanged, new Action<GameLogic.GameState>(OnGameStateChanged));
        }

        private void OnGameStateChanged(GameLogic.GameState state)
        {
            target.SetActive(states.Contains(state));
        }
    }
}
