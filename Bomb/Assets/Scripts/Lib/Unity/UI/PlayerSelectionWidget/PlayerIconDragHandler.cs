using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;


namespace Lib.Unity.UI.PlayerSelectionWidget
{
    public enum State
    {
        None = 0,
        Returning = 1,
        Swapping = 2,
    }
    
    public class PlayerIconDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public PlayerSelectionWidget playerSelectionWidget;
        public Vector3 startPosition;
        private int _siblingIndex;

        private Vector3 _offset;
        private RectTransform _rectTransform;
        private State _state = State.None;

        public PlayerIconDragHandler targetDragHandler;
        private AnimationCurve curve;
        private float swappingTimer = 0;

        private bool _isHolding = false;
        private float _holdingTimer = 0;
        private readonly float _holdingTime = 2f;

        protected virtual void OnHolded() { }

        public void setState(State state)
        {
            if (_state != state)
            {
                _state = state;
                if (state == State.Swapping || state == State.Returning)
                {
                    swappingTimer = 0;
                    curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                }
            }
        }

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Start()
        {
            startPosition = _rectTransform.position;
            _siblingIndex = transform.GetSiblingIndex();
        }
        
        public void ResetPosition(Vector3 position)
        {
            startPosition = position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _siblingIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();

            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint);

            _offset = _rectTransform.position - worldPoint;
            
            _isHolding = true;
            _holdingTimer = 0;
            
            Begin();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    _rectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector3 worldPoint))
            {
                _rectTransform.position = worldPoint + _offset;

                if (_isHolding && Vector3.Distance(_rectTransform.position, startPosition) > 0.1f)
                {
                    _isHolding = false;
                }
                
                
                Rect droppedRect = GetWorldRect(GetComponent<RectTransform>());
                var dragHandler = gameObject.GetComponent<PlayerIconDragHandler>();
                bool isHold = false;
                foreach (GameObject icon in playerSelectionWidget.playerIcons)
                {
                    if (icon != gameObject)
                    {
                        var iconDragHandler = icon.GetComponent<PlayerIconDragHandler>();
                        var startPosition = iconDragHandler.startPosition;
                        var rect = icon.GetComponent<RectTransform>().rect;
                        Rect targetRect = new Rect(startPosition.x - rect.width / 2, startPosition.y - rect.height / 2, rect.width, rect.height);
                        if (!isHold && droppedRect.Overlaps(targetRect))
                        {
                            isHold = true;
                            iconDragHandler.setState(State.Swapping);
                            iconDragHandler.targetDragHandler = dragHandler;
                        }
                        else if (iconDragHandler.targetDragHandler != null)
                        {
                            iconDragHandler.setState(State.Returning);
                            iconDragHandler.targetDragHandler = null;
                        }
                    }
                }

                Rect addPlayerButtonRect = GetWorldRect(playerSelectionWidget.addPlayerButton.GetComponent<RectTransform>());
                if (droppedRect.Overlaps(addPlayerButtonRect) && playerSelectionWidget.IsInteractable())
                {
                    playerSelectionWidget.addPlayerButton.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    playerSelectionWidget.addPlayerButton.GetComponent<Outline>().enabled = false;
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isHolding = false;
            transform.SetSiblingIndex(_siblingIndex);
            playerSelectionWidget.addPlayerButton.GetComponent<Outline>().enabled = false;
            
            Rect droppedRect = GetWorldRect(GetComponent<RectTransform>());
            foreach (GameObject icon in playerSelectionWidget.playerIcons)
            {
                if (icon != gameObject)
                {
                    var iconDragHandler = icon.GetComponent<PlayerIconDragHandler>();
                    var startPosition = iconDragHandler.startPosition;
                    var rect = icon.GetComponent<RectTransform>().rect;
                    Rect targetRect = new Rect(startPosition.x - rect.width / 2, startPosition.y - rect.height / 2, rect.width, rect.height);
                    if (droppedRect.Overlaps(targetRect))
                    {
                        playerSelectionWidget.SwapPlayerPositions(gameObject, icon);
                        gameObject.GetComponent<PlayerIconDragHandler>().setState(State.None);
                        iconDragHandler.setState(State.None);
                        End();
                        return;
                    }
                }
            }

            Rect addPlayerButtonRect =
                GetWorldRect(playerSelectionWidget.addPlayerButton.GetComponent<RectTransform>());
            if (droppedRect.Overlaps(addPlayerButtonRect) && playerSelectionWidget.IsInteractable())
            {
                playerSelectionWidget.RemovePlayer(gameObject);
                End();
                return;
            }

            setState(State.Returning);
            End();
        }

        protected virtual void Begin()
        {
            
        }
        protected virtual void End()
        {
            
        }

        void Update()
        {
            if (_isHolding)
            {
                _holdingTimer += Time.deltaTime;
                if (_holdingTimer >= _holdingTime)
                {
                    _isHolding = false;
                    OnHolded();
                }
            }
            
            switch (_state)
            {
                case State.Swapping:
                    swappingTimer += Time.deltaTime;
                    var time = curve.Evaluate(swappingTimer);
                    var targetRectTransform = targetDragHandler.GetComponent<RectTransform>();
                    _rectTransform.position = Vector3.Lerp(_rectTransform.position, targetDragHandler.startPosition, time);
                    break;
                case State.Returning:
                    if (Vector3.Distance(_rectTransform.position, startPosition) < 0.01)
                    {
                        _rectTransform.position = startPosition;
                        _state = State.None;
                    }
                    else
                    {
                        swappingTimer += Time.deltaTime;
                        _rectTransform.position = Vector3.Lerp(_rectTransform.position, startPosition, curve.Evaluate(swappingTimer));
                    }
                    break;
            }
        }

        private Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }
    }
}