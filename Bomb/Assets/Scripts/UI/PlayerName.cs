using Common;
using GameLogic;
using System;
using UnityEngine.UI;

namespace UI
{
    public class PlayerName : GameObserverMonoBehaviour
    {
        TMPro.TextMeshProUGUI textComponent;

        protected override void Subscribe()
        {
            _eventListener.Add(Events.EvCurrentPlayerChanged, new Action(OnCurrentPlayerChanged));
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnGameStateChanged));
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            textComponent = GetComponent<TMPro.TextMeshProUGUI>();
        }

        protected void OnCurrentPlayerChanged()
        {
            UpdateState(GameComponent.State);
        }

        void OnGameStateChanged(GameState state)
        {
            UpdateState(state);
        }

        private void UpdateState(GameState state)
        {
            if (state == GameState.Play || state == GameState.Countdown || state == GameState.Explosion || state == GameState.ReadyToStart)
            {
                textComponent.text = GameComponent.GetCurrentPlayer().Name;
            }
            else
            {
                textComponent.text = "";
            }
        }
    }
}
