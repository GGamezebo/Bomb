using System;
using Common;
using GameLogic;
using UnityEngine;

namespace UI.HUD
{
    public class PlayerName : GameObserverMonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI textComponent;
        [SerializeField] private Game game;

        protected override void Subscribe()
        {
            EventListener.Add(Events.EvCurrentPlayerChanged, new Action<Player>(OnCurrentPlayerChanged));
        }

        private void OnCurrentPlayerChanged(Player currentPlayer)
        {
            UpdatePlayerName();
        }

        private void UpdatePlayerName()
        {
            textComponent.text = game.GetCurrentPlayer().Name;
        }
    }
}