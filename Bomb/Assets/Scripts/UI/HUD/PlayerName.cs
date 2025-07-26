using System;
using Common;
using GameLogic;
using UnityEngine;

namespace UI.HUD
{
    public class PlayerName : GameObserverMonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI textComponent;

        protected override void Subscribe()
        {
            EventListener.Add(Events.EvCurrentPlayerChanged, new Action<Player>(OnCurrentPlayerChanged));
        }


        private void OnCurrentPlayerChanged(Player currentPlayer)
        {
            textComponent.text = currentPlayer.Name;
        }
    }
}