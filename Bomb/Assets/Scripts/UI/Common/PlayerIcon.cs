using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerIcon : MonoBehaviour
    {
        [Header("Player Icon")]
        [SerializeField] private Image image;
        [SerializeField] private TMPro.TextMeshProUGUI text;

        void Start()
        {
            text.color = Color.black;
            text.transform.localPosition = new Vector2(0, -image.rectTransform.rect.height / 2 - 25); // = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + rect.rect.height);
        }
        
    }
}