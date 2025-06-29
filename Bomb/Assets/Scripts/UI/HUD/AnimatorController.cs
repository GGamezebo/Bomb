using UnityEngine;
using Common;
using System;
using GameLogic;

namespace UI.HUD
{
    public enum AnimStates
    {
        None,
        Ready,
        Comes,
        Alert,
        Boom
    }

    public class AnimatorController : GameObserverMonoBehaviour
    {
        [SerializeField] private Animator bombAnimator;
        [SerializeField] private Animator backgroundAnimator;

        private Animator[] _animators;
        
        protected override void Subscribe()
        {
            _eventListener.Add(Events.EvGameStateChanged, new Action<GameState>(OnGameStateChanged));
            _eventListener.Add(Events.EvAlert, new Action(OnAlert));
        }
        
        private void Start()
        {
            _animators = new[] { bombAnimator, backgroundAnimator};
        }

        void OnGameStateChanged(GameState state)
        {
            AnimStates animState = AnimStates.None;
            if (state == GameState.Play)
            {
                animState = AnimStates.Comes;
            }
            else if (state == GameState.ReadyToStart)
            {
                animState = AnimStates.Ready;
            }
            else if (state == GameState.Explosion)
            {
                animState = AnimStates.Boom;
            }

            if (animState != AnimStates.None)
            {
                SetTrigger(animState);
            }
        }

        void OnAlert()
        {
            SetTrigger(AnimStates.Alert);
        }
        
        private void SetTrigger(AnimStates state)
        {
            var triggerName = state.ToString();
            foreach (var animator in _animators) 
            { 
                animator.SetTrigger(triggerName); 
            };
        }
    }
}