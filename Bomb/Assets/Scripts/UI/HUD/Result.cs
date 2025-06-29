using System;
using Common;
using GameLogic;
using UnityEngine;

namespace UI.HUD
{
    public class Result : GameObserverMonoBehaviour
    {
        [SerializeField] private Game gameComponent;
        TMPro.TextMeshProUGUI textComponent;


        protected override void Subscribe()
        {
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnGameStateChanged));
        }

        // Start is called before the first frame update
        private void Start()
        {
            textComponent = GetComponent<TMPro.TextMeshProUGUI>();
        }

        void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Result)
            {
                var result = gameComponent.GetResult();
                string text = $"Победил {result[0].Name}!\n\n";
                foreach(Player player in result)
                {
                    text = text + $"{player.Name}:\t\t{player.Score}\n";
                }

                textComponent.text = text;
            }
            else
            {
                textComponent.text = "";
            }
        }
    }
}
