using Common;
using GameLogic;
using UnityEngine.UI;

namespace UI
{
    public class Result : GameObserverMonoBehaviour
    {
        TMPro.TextMeshProUGUI textComponent;
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            textComponent = GetComponent<TMPro.TextMeshProUGUI>();
        }

        protected override void OnStateChanged(GameState state)
        {
            if (state == GameState.Result)
            {
                var result = GameComponent.GetResult();
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
