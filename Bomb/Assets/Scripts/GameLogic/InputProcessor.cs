using Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
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
        
        [SerializeField] public InputActionAsset uiActions;

        protected override void Awake()
        {
            base.Awake();
            _gameInput = new GameInput();
            
            EnhancedTouchSupport.Enable();
        }
        
        protected override void OnEnable()
        {
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += OnFingerUp;
            base.OnEnable();
            _gameInput.Enable();
        }
        
        protected override void OnDisable()
        {
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= OnFingerUp;
            _gameInput.Disable();
            base.OnDisable();
        }
        
        void OnFingerUp(Finger finger)
        {
            // gameComponent.StartRound();
            // _event.Call(Events.EvTouchStartRound);
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
                ProcessTouchInput();
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
                // if (_clickAction.WasReleasedThisFrame())
                // {
                //     gameComponent.StartRound();
                //     _event.Call(Events.EvTouchStartRound);
                // }
            }
            else if (gameComponent.State == GameState.Result)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
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
                        // ���������, ��� ��� ������ ������
                        bool noTouches = true;
                        foreach (var touch in Touchscreen.current.touches)
                        {
                            if (touch.isInProgress) // ���� ���� �������� �������
                            {
                                noTouches = false;
                                break;
                            }
                        }

                        if (noTouches)
                        {
                            gameComponent.NextPlayer();
                            _event.Call(Events.EvTouchNextPlayer);
                        }
                    }
                }

                if (_clickAction.WasPressedThisFrame())
                {
                    _position = _pointAction.ReadValue<Vector2>();
                }
            }
            else if (gameComponent.State == GameState.ReadyToStart)
            {
                // if (_clickAction.WasReleasedThisFrame())
                // {
                //     gameComponent.StartRound();
                //     _event.Call(Events.EvTouchStartRound);
                // }
            }
            else if (gameComponent.State == GameState.Result)
            {
                if (_clickAction.WasReleasedThisFrame())
                {
                    SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
                }
            }
        }
    }
}