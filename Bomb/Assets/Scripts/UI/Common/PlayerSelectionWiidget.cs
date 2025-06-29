using Common;
using System.Linq;
using Account;

namespace UI.Common
{
    public class PlayerSelectionWidget : Lib.Unity.UI.PlayerSelectionWidget.PlayerSelectionWidget
    {
        private GlobalContext _globalContext;
        private Lib.Event _event;
        private Lib.EventListener _eventListener;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _globalContext = FindFirstObjectByType<GlobalContext>();
            _event = _globalContext.MakeEvent();
            _eventListener = _globalContext.MakeEventListener();
        }

        protected override void OnDisable()
        {
            _eventListener.RemoveAllListeners();
            base.OnDisable();
        }
        
        private void Start()
        {
            PlayerNames = _globalContext.PData().players.Select(player => player.name).ToList();
            foreach (var playerName in PlayerNames)
            {
                CreatePlayerIcon(playerName);
            }
            UpdatePlayerPositions();
        }
        
        protected override void OnPlayerAdded(string playerName)
        {
            var playerInfo = new PlayerInfo(playerName, 0);
            _globalContext.PData().players.Add(playerInfo);
            _globalContext.accountDataComponent.Save();
            _event.Call(Events.EvPlayerAdded, playerInfo);
        }
        
        protected override void OnPlayerRemoved(int playerIndex)
        {
            var playerInfo = _globalContext.PData().players[playerIndex]; 
            _globalContext.PData().players.RemoveAt(playerIndex);
            _globalContext.accountDataComponent.Save();
            _event.Call(Events.EvPlayerRemoved, playerInfo);
        }
    }
}