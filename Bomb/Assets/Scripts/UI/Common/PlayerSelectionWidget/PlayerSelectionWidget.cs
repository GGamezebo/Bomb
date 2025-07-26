using System;
using Account;
using Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common.PlayerSelectionWidget
{
    public class PlayerSelectionWidget : Lib.Unity.UI.PlayerSelectionWidget.PlayerSelectionWidget
    {
        [SerializeField] private GlobalContext globalContext;
        [SerializeField] private Sprite addPlayerSprite;
        [SerializeField] private Sprite removePlayerSprite;
        [SerializeField] private int minPlayersForRemoving;
        
        private Lib.Event _event;
        private Lib.EventListener _eventListener;

        protected override void Awake()
        {
            _event = globalContext.MakeEvent();
            _eventListener = globalContext.MakeEventListener();
        }
        
        private void Start()
        {
            foreach (var playerInfo in globalContext.PData().players)
            {
                CreatePlayerIcon(playerInfo.name, playerInfo.presetId);
            }
            UpdatePlayerPositions();
            UpdateAddPlayerButton();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _eventListener.Add(Events.EvPlayerMoveBegin, new Action(OnPlayerMoveBegin));
            _eventListener.Add(Events.EvPlayerMoveEnd, new Action(OnPlayerMoveEnd));
        }

        protected override void OnDisable()
        {
            _eventListener.RemoveAllListeners();
            base.OnDisable();
        }
        
        protected override void AddComponent(GameObject newIcon)
        {
            newIcon.AddComponent<UI.Common.PlayerSelectionWidget.PlayerIconDragHandler>();
        }

        private void OnPlayerMoveBegin()
        {
            addPlayerButton.GetComponent<Image>().sprite = removePlayerSprite;
            UpdateAddPlayerButton();
        }
        
        private void OnPlayerMoveEnd()
        {
            addPlayerButton.GetComponent<Image>().sprite = addPlayerSprite;
            UpdateAddPlayerButton();
        }
        
        protected override void OnPlayerAdded(string playerName, int presetId)
        {
            var playerInfo = new PlayerInfo(playerName, presetId);
            globalContext.PData().players.Add(playerInfo);
            globalContext.accountDataComponent.Save();
            UpdateAddPlayerButton();
            _event.Call(Events.EvPlayerAdded, playerInfo);
        }
                
        protected override void OnPlayerModified(int playerIndex, string playerName, int presetId)
        {
            var playerInfo = globalContext.PData().players[playerIndex];
            playerInfo.name = playerName;
            playerInfo.presetId = presetId;
            globalContext.accountDataComponent.Save();
            _event.Call(Events.EvPlayerModified, playerInfo);
        }
        
        protected override void OnPlayerRemoved(int playerIndex)
        {
            var playerInfo = globalContext.PData().players[playerIndex]; 
            globalContext.PData().players.RemoveAt(playerIndex);
            globalContext.accountDataComponent.Save();
            UpdateAddPlayerButton();
            _event.Call(Events.EvPlayerRemoved, playerInfo);
        }        
        protected override void OnSwapPlayerPositions(int index1, int index2)
        {
            var players = globalContext.PData().players;
            (players[index1], players[index2]) = (players[index2], players[index1]);
            globalContext.accountDataComponent.Save();
            UpdateAddPlayerButton();
            _event.Call(Events.EvPlayerSwapped);
        }

        private bool IsFullPlayers()
        {
            return globalContext.PData().players.Count >= globalContext.gameSettings.maxPlayers;
        }       
        
        private bool IsMinPlayers()
        {
            return globalContext.PData().players.Count <= minPlayersForRemoving;
        }

        private void UpdateAddPlayerButton()
        {
            addPlayerButton.GetComponent<Button>().interactable = IsInteractable();
        }
        
        public override bool IsInteractable()
        {
            bool interactable = true;
            if (addPlayerButton.GetComponent<Image>().sprite == addPlayerSprite)
            {
                interactable = !IsFullPlayers();
            }
            else if (addPlayerButton.GetComponent<Image>().sprite == removePlayerSprite)
            {
                interactable = !IsMinPlayers();
            }

            return interactable;
        }
        
    }
}