using Events = Common.Events;

namespace UI.Common.PlayerSelectionWidget
{
    public class PlayerIconDragHandler : Lib.Unity.UI.PlayerSelectionWidget.PlayerIconDragHandler
    {
        private GlobalContext _globalContext;
        private Lib.Event _event;
        
        protected override void Awake()
        {
            base.Awake();
            _globalContext = FindFirstObjectByType<GlobalContext>();
            _event = _globalContext.MakeEvent();
        }

        protected override void Begin()
        {
            _event.Call(Events.EvPlayerMoveBegin);
        }
        
        protected override void End()
        {
            _event.Call(Events.EvPlayerMoveEnd);
        }

        protected override void OnHolded()
        {
            int index = playerSelectionWidget.playerIcons.IndexOf(gameObject);
            var playerInfo = _globalContext.PData().players[index];
            playerSelectionWidget.OpenEditWindow(index, playerInfo.name, playerInfo.presetId);
        }
    }
}