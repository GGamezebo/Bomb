using System.Collections.Generic;
using Common;
using Lib.Unity.UI;


namespace UI
{
    public class PlayerSelectionWidget : TGPlayerSelectionWidget
    {
        private GlobalContext _globalContext;
        private Lib.Event _event;
        private Lib.EventListener _eventListener;
        
        private void OnEnable()
        {
            _globalContext = FindFirstObjectByType<GlobalContext>();
            _event = _globalContext.MakeEvent();
            _eventListener = _globalContext.MakeEventListener();
        }
        
        protected override void Start()
        {
            base.Start();
            PlayerNames = _globalContext.PData().playerNames;
            foreach (var playerName in PlayerNames)
            {
                CreatePlayerIcon(playerName);
            }
            UpdatePlayerPositions();
        }
        
        protected override void OnPlayerAdded(string playerName)
        {
            _globalContext.accountData.Save();
            _event.Call(Events.EvPlayerAdded, playerName);
        }
        
        protected override void OnPlayerRemoved(string playerName)
        {
            _globalContext.accountData.Save();
            _event.Call(Events.EvPlayerRemoved, playerName);
        }
    }
}