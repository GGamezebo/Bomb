using GameLogic;
using UnityEngine;
using Common;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;

namespace UI.HUD
{
    public class ReadyToStart : MonoBehaviour
    {
        [SerializeField] private Game gameComponent;
        [SerializeField] private GlobalContext globalContext;
        private Lib.Event _event;

        private void Awake()
        {
            _event = globalContext.MakeEvent();
        }
        
        public void OnStartRound()
        {
            gameComponent.StartRound();
            _event.Call(Events.EvTouchStartRound);
        }

        // protected void OnEnable()
        // {
        //     //UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += OnFingerUp;
        // }
        //
        // protected void OnDisable()
        // {
        //     //UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= OnFingerUp;
        // }
        
    }
}