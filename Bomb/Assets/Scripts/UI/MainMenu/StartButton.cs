#if UNITY_ANDROID 
#endif
using System;
using Account;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class StartButton : MonoBehaviour
    {
        private GlobalContext _globalContext;
        private Lib.EventListener _eventListener;
        
        private void OnEnable()
        {
            _globalContext = FindFirstObjectByType<GlobalContext>();
            _eventListener = _globalContext.MakeEventListener();
            _eventListener.Add(Events.EvPlayerAdded, new Action<PlayerInfo>(OnAddPlayer));
            _eventListener.Add(Events.EvPlayerRemoved, new Action<PlayerInfo, int>(OnRemovePlayer));
        }
        
        private void OnDisable()
        {
            _eventListener.RemoveAllListeners();
        }

        private void UpdateState()
        {
            var isActive = _globalContext.PData().players.Count >= _globalContext.gameSettings.minPlayers;
            var button = GetComponent<Button>();
            button.interactable = isActive;
        }
        
        void Start()
        {
            UpdateState();
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnStartGame);
        }

        private void OnAddPlayer(PlayerInfo playerName)
        {
            UpdateState();
        }

        private void OnRemovePlayer(PlayerInfo playerName, int playerIndex)
        {
            UpdateState();
        }

        void OnStartGame()
        {
            SceneManager.LoadScene(Scenes.Game);
        }
    }
}
