using System;
using Common;
using GameLogic;
using UnityEngine;

namespace UI.HUD
{
    public class PlayerName : GameObserverMonoBehaviour
    {
        [SerializeField] private Game gameComponent;
        TMPro.TextMeshProUGUI textComponent;

        protected override void Subscribe()
        {
            _eventListener.Add(Events.EvCurrentPlayerChanged, new Action(OnCurrentPlayerChanged));
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnGameStateChanged));
        }

        // Start is called before the first frame update
        private void Start()
        {
            textComponent = GetComponent<TMPro.TextMeshProUGUI>();
        }

        private void OnCurrentPlayerChanged()
        {
            UpdateState(gameComponent.State);
        }

        private void OnGameStateChanged(GameState state)
        {
            UpdateState(state);
        }

        private void UpdateState(GameState state)
        {
            if (state == GameState.Play || state == GameState.Countdown || state == GameState.Explosion || state == GameState.ReadyToStart)
            {
                textComponent.text = gameComponent.GetCurrentPlayer().Name;
            }
            else
            {
                textComponent.text = "";
            }
        }
    }
}
