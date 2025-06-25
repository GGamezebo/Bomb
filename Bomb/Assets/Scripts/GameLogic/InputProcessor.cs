using Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class InputProcessor : GameObserverMonoBehaviour
    {
        [SerializeField] private Game gameComponent;
        
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

        protected void Start()
        {
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
            if (gameComponent.State == GameState.Play)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    gameComponent.NextPlayer();
                    _event.Call(Events.EvTouchNextPlayer);
                }

                if (_rightClickAction.WasReleasedThisFrame())
                {
                    gameComponent.PrevPlayer();
                    _event.Call(Events.EvTouchPrevPlayer);
                }
            }
            else if (gameComponent.State == GameState.ReadyToStart)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    gameComponent.StartRound();
                    _event.Call(Events.EvTouchStartRound);
                }
            }
            else if (gameComponent.State == GameState.Result)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }
            }
        }

        private void ProcessTouchInput()
        {
            if (gameComponent.State == GameState.Play)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    Vector2 currentPosition = _pointAction.ReadValue<Vector2>();
                    if ((_position - currentPosition).magnitude > 650)
                    {
                        gameComponent.PrevPlayer();
                        _event.Call(Events.EvTouchPrevPlayer);
                    }
                    else
                    {
                        gameComponent.NextPlayer();
                        _event.Call(Events.EvTouchNextPlayer);
                    }
                }

                if (_clickAction.WasPressedThisFrame())
                {
                    _position = _pointAction.ReadValue<Vector2>();
                }
            }
            else if (gameComponent.State == GameState.ReadyToStart)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    gameComponent.StartRound();
                    _event.Call(Events.EvTouchStartRound);
                }
            }
            else if (gameComponent.State == GameState.Result)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                }
            }
        }
    }
}