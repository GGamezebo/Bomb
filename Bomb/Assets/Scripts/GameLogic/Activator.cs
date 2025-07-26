using System;
using System.Collections.Generic;
using UnityEngine;


namespace Common
{
    public sealed class Activator : GameObserverMonoBehaviour
    {
        [SerializeField] private List<GameLogic.GameState> States;
        
        protected override void Subscribe()
        {
            EventListener.Add(Events.EvGameStateChanged, new Action<GameLogic.GameState>(OnGameStateChanged));
        }

        private void OnGameStateChanged(GameLogic.GameState state)
        {
            gameObject.SetActive(States.Contains(state));
        }
    }
}
