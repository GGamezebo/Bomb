using System;
using Common;
using GameLogic;
using UnityEngine;

namespace UI.HUD
{
    public class Information : GameObserverMonoBehaviour
    {
        [SerializeField] private Game gameComponent;
        
        TMPro.TextMeshProUGUI textComponent;

        protected override void Subscribe()
        {
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnGameStateChanged));
        }
        private void Start()
        {
            textComponent = GetComponent<TMPro.TextMeshProUGUI>();
        }

        void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Play)
            {
                var card = gameComponent.CurrentCard;
                string place;
                if (card.Condition == WordCondition.Begin)
                {
                    place = "В начале слова";
                }
                else if (card.Condition == WordCondition.Anywhere)
                {
                    place = "Где угодно";
                }
                else
                {
                    place = "В конце слова";
                }

                textComponent.text = string.Format("<size=60>({1})</size>\n<size=90>{0}</size>", card.Word, place);
            }
            else if (state == GameState.Explosion)
            {
                var player = gameComponent.GetCurrentPlayer();
                textComponent.text = $"<size=75>Игрок: {player.Name}</size>\n<size=39>Вас подорвало!</size>";
            }
            else if (state == GameState.ReadyToStart)
            {
                var player = gameComponent.GetCurrentPlayer();
                textComponent.text = "Нажми, чтобы начать следующий раунд!";
            }
            else
            {
                textComponent.text = "";
            }
        }
    }
}
