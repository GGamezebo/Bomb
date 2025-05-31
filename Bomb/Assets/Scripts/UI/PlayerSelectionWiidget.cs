using System.Collections.Generic;
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
            PlayerColors = _globalContext.AccountData.gameSettings.colors;
            foreach (var playerName in PlayerNames)
            {
                CreatePlayerIcon(playerName);
            }
            UpdatePlayerPositions();
        }
        
        protected override void SendEvent(string eventName, params object[] args)
        {
            _event.Call(eventName, args);
        }
    }
}