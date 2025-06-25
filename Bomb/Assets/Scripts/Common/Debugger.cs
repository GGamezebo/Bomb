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

        void OnAlert()
        {
            Debug.Log("<><><> onAlert");
        }

        void OnCurrentPlayerChanged()
        {
            Debug.Log("<><><> onCurrentPlayerChanged " + _gameComponent.currentPlayerIndex);
        }

        void Subscribe()
        {
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnStateChanged));
            _eventListener.Add(Events.EvCurrentPlayerChanged, new Action(OnCurrentPlayerChanged));
            _eventListener.Add(Events.EvAlert, new Action(OnAlert));
        }
    }
}