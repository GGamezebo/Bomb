using System;
using GameLogic;
using UnityEngine;

namespace Common
{
    public class GameObserverMonoBehaviour : MonoBehaviour
    {
        
        void OnDestroy()
        {
            _eventListener = null;
            GameComponent = null;
        }

        protected Game GameComponent;
        protected Lib.EventListener _eventListener;

        protected virtual void OnEnable()
        {
            var globalContext = FindFirstObjectByType<GlobalContext>();
            _eventListener = globalContext.MakeEventListener();
            if (this.didStart)
            {
                Subscribe();
            }
        }

        protected virtual void OnDisable()
        {
            _eventListener.RemoveAllListeners();
        }

        protected virtual void Start()
        {
            var game = GameObject.Find("Game");
            GameComponent = game.GetComponent<Game>();
            Subscribe();
        }

        protected virtual void OnStateChanged(GameState state)
        {
            
        }

        protected virtual void OnCurrentPlayerChanged()
        {

        }

        protected virtual void OnAlert()
        {

        }

        private void Subscribe()
        {
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnStateChanged));
            _eventListener.Add(Events.EvCurrentPlayerChanged, new Action(OnCurrentPlayerChanged));
            _eventListener.Add(Events.EvAlert, new Action(OnAlert));
        }
    }
}