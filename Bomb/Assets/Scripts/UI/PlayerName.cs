using Common;
using GameLogic;
using UnityEngine.UI;

namespace UI
{
    public class PlayerName : GameObserverMonoBehaviour
    {
        TMPro.TextMeshProUGUI textComponent;
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            textComponent = GetComponent<TMPro.TextMeshProUGUI>();
        }

        protected override void OnCurrentPlayerChanged()
        {
            UpdateState(GameComponent.State);
        }

        protected override void OnStateChanged(GameState state)
        {
            UpdateState(state);
        }

        private void UpdateState(GameState state)
        {
            if (state == GameState.Play || state == GameState.Countdown || state == GameState.Explosion || state == GameState.ReadyToStart)
            {
                textComponent.text = GameComponent.getCurrentPlayer().Name;
            }
            else
            {
                textComponent.text = "";
            }
        }
    }
}
