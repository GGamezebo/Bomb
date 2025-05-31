using System;
using GameLogic;
using UnityEngine;
using Lib;

namespace Common
{
    public class Debugger : MonoBehaviour
    {
        private EventListener _eventListener;
        private Game _gameComponent;

        void OnEnable()
        {
            var globalContext = FindFirstObjectByType<GlobalContext>();
            _eventListener = globalContext.MakeEventListener();
            if (didStart)
            {
                Subscribe();
            }
        }

        void OnDisable()
        {
            _eventListener.RemoveAllListeners();
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("<><><> Debugger: Start");
            var game = GameObject.Find("Game");
            _gameComponent = game.GetComponent<Game>();

            Subscribe();
        }
        

        void OnStateChanged(GameState state)
        {
            Debug.Log("<><><> onStateChanged " + state.ToString());
        }

        void onAlert()
        {
            Debug.Log("<><><> onAlert");
        }

        void onCurrentPlayerChanged()
        {
            Debug.Log("<><><> onCurrentPlayerChanged " + _gameComponent.currentPlayerIndex);
        }

        void Subscribe()
        {
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnStateChanged));
            _eventListener.Add(Events.EvCurrentPlayerChanged, new Action(onCurrentPlayerChanged));
            _eventListener.Add(Events.EvAlert, new Action(onAlert));
        }
    }
}