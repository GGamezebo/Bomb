using Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class InputProcessor : GameObserverMonoBehaviour
    {
        private GameInput _gameInput;
        private InputAction _pointAction;
        private InputAction _clickAction;
        private InputAction _rightClickAction;
        
        private Lib.Event _event;
        private Vector2 _position;

        private void Awake()
        {
            _gameInput = new GameInput();
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _gameInput.Enable();
        }

        protected override void OnDisable()
        {
            _gameInput.Disable();
            base.OnDisable();
        }

        protected override void Start()
        {
            base.Start();
            
            _clickAction = _gameInput.UI.Click;
            _rightClickAction = _gameInput.UI.RightClick;
            _pointAction = _gameInput.UI.Point;

            var globalContext = FindFirstObjectByType<GlobalContext>();
            _event = globalContext.MakeEvent();
        }

        void Update()
        {
            if (Application.isMobilePlatform)
            {
                ProcessTouchInput();
            }

            else if (Application.platform == RuntimePlatform.WindowsPlayer || 
                     Application.platform == RuntimePlatform.WindowsEditor)
            {
                ProcessMouseInput();
            }
        }

        private void ProcessMouseInput()
        {
            if (GameComponent.State == GameState.Play)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    GameComponent.nextPlayer();
                    _event.Call(Events.EvTouchNextPlayer);
                }

                if (_rightClickAction.WasReleasedThisFrame())
                {
                    GameComponent.prevPlayer();
                    _event.Call(Events.EvTouchPrevPlayer);
                }
            }
            else if (GameComponent.State == GameState.ReadyToStart)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    GameComponent.startRound();
                    _event.Call(Events.EvTouchStartRound);
                }
            }
            else if (GameComponent.State == GameState.Result)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }
            }
        }

        private void ProcessTouchInput()
        {
            if (GameComponent.State == GameState.Play)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    Vector2 currentPosition = _pointAction.ReadValue<Vector2>();
                    if ((_position - currentPosition).magnitude > 650)
                    {
                        GameComponent.prevPlayer();
                        _event.Call(Events.EvTouchPrevPlayer);
                    }
                    else
                    {
                        GameComponent.nextPlayer();
                        _event.Call(Events.EvTouchNextPlayer);
                    }
                }

                if (_clickAction.WasPressedThisFrame())
                {
                    _position = _pointAction.ReadValue<Vector2>();
                }
            }
            else if (GameComponent.State == GameState.ReadyToStart)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    GameComponent.startRound();
                    _event.Call(Events.EvTouchStartRound);
                }
            }
            else if (GameComponent.State == GameState.Result)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }
            }
        }
    }
}