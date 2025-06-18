#if UNITY_ANDROID 
#endif
using System;
using Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class StartButton : MonoBehaviour
    {
        public Sprite activeSprite;
        public Sprite inactiveSprite;
        private GlobalContext _globalContext;
        private Lib.EventListener _eventListener;
        
        private void OnEnable()
        {
            _globalContext = FindFirstObjectByType<GlobalContext>();
            _eventListener = _globalContext.MakeEventListener();
            _eventListener.Add(Events.EvPlayerAdded, new Action<string>(OnAddPlayer));
            _eventListener.Add(Events.EvPlayerRemoved, new Action<string>(OnRemovePlayer));
        }
        
        private void OnDisable()
        {
            _eventListener.RemoveAllListeners();
        }

        private void UpdateState()
        {
            var isActive = _globalContext.PData().players.Count >= 2;
            var button = GetComponent<Button>();
            button.interactable = isActive;
            var image = GetComponent<Image>();
            image.sprite = isActive ? activeSprite : inactiveSprite;
        }
        
        void Start()
        {
            UpdateState();
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnStartGame);
        }

        private void OnAddPlayer(string playerName)
        {
            UpdateState();
        }

        private void OnRemovePlayer(string playerName)
        {
            UpdateState();
        }

        void OnStartGame()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
