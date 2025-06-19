using System;
using GameLogic;
using UnityEngine;
using Common;

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
            Subscribe();
        }

        protected virtual void OnDisable()
        {
            _eventListener.RemoveAllListeners();
        }

        protected virtual void Start()
        {
            var game = GameObject.Find("Game");
            GameComponent = game.GetComponent<Game>();
        }

        protected virtual void Subscribe()
        {

        }
    }
}