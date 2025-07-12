using UnityEngine;
using UnityEngine.EventSystems;


namespace Lib.Unity.UI.PlayerSelectionWidget
{
    public class PlayerIconDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public PlayerSelectionWidget playerSelectionWidget;
        public Vector3 startPosition;
        private int _siblingIndex;

        private Vector3 _offset;
        private RectTransform _rectTransform;
        private bool _isReturning = false;
        private readonly float _returnSpeed = 5f;

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Start()
        {
            startPosition = _rectTransform.localPosition;
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
            
            _isReturning = false;

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
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.SetSiblingIndex(_siblingIndex);
            _isReturning = true;

            Rect droppedRect = GetWorldRect(GetComponent<RectTransform>());
            foreach (GameObject icon in playerSelectionWidget.playerIcons)
            {
                if (icon != gameObject)
                {
                    Rect targetRect = GetWorldRect(icon.GetComponent<RectTransform>());
                    if (droppedRect.Overlaps(targetRect))
                    {
                        playerSelectionWidget.SwapPlayerPositions(gameObject, icon);
                        End();
                        return;
                    }
                }
            }

            Rect addPlayerButtonRect =
                GetWorldRect(playerSelectionWidget.addPlayerButton.GetComponent<RectTransform>());
            if (droppedRect.Overlaps(addPlayerButtonRect))
            {
                playerSelectionWidget.RemovePlayer(gameObject);
                End();
                return;
            }

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
            if (_isReturning && Vector3.Distance(_rectTransform.localPosition, startPosition) > 0.01f)
            {
                _rectTransform.localPosition = Vector3.Lerp(_rectTransform.localPosition, startPosition,
                    _returnSpeed * Time.deltaTime);
            }
            else
            {
                _isReturning = false;
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