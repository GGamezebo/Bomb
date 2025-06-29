using UnityEngine;
using UnityEngine.EventSystems;

namespace Lib.Unity.UI.PlayerSelectionWidget
{
    public class PlayerIconDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
        {
            public PlayerSelectionWidget playerSelectionWidget; // Ссылка на основной виджет
            public Vector3 startPosition;
            private int siblingIndex;

            public void Start()
            {
                startPosition = GetComponent<RectTransform>().anchoredPosition;
                siblingIndex = transform.GetSiblingIndex();
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                transform.SetAsLastSibling();
                //playerSelectionWidget.StartDragging(gameObject);
                //  GetComponent<CanvasGroup>().blocksRaycasts = false;
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                transform.SetSiblingIndex(siblingIndex);
                //GetComponent<CanvasGroup>().blocksRaycasts = true;
                //playerSelectionWidget.StopDragging(gameObject);

                Rect droppedRect = GetWorldRect(GetComponent<RectTransform>());

                foreach (GameObject icon in playerSelectionWidget.playerIcons)
                {
                    if (icon != gameObject)
                    {
                        Rect targetRect = GetWorldRect(icon.GetComponent<RectTransform>());
                        if (droppedRect.Overlaps(targetRect))
                        {
                            playerSelectionWidget.SwapPlayerPositions(gameObject, icon);
                            return;
                        }
                    }
                }

                Rect addPlayerButtonRect =
                    GetWorldRect(playerSelectionWidget.addPlayerButton.GetComponent<RectTransform>());
                if (droppedRect.Overlaps(addPlayerButtonRect))
                {
                    playerSelectionWidget.RemovePlayer(gameObject);
                    return;
                }

                GetComponent<RectTransform>().anchoredPosition = startPosition;
            }

            private Rect GetWorldRect(RectTransform rectTransform)
            {
                Vector3[] corners = new Vector3[4];
                rectTransform.GetWorldCorners(corners);
                return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
            }

            public void OnDrag(PointerEventData eventData)
            {
                var rectTransform = GetComponent<RectTransform>();
                var canvas = GetComponentInParent<Canvas>();

                //transform.position = eventData.position;
                rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }
        }
       
}