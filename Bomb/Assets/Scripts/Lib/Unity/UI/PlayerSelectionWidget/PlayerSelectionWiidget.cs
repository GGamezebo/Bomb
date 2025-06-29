using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lib.Unity.UI.PlayerSelectionWidget
{
    public class PlayerSelectionWidget : MonoBehaviour
    {
        [SerializeField] public GameObject addPlayerButton;

        public GameObject modalPanel; // Панель модального окна
        public TMP_InputField playerNameInputField; // Поле ввода имени игрока (TextMeshPro)
        public GameObject playerIconPrefab; // Префаб иконки игрока (TextMeshPro - TMP_Text)
        public float iconScale = 1.0f; // Масштаб иконок

        public List<GameObject> playerIcons = new List<GameObject>(); // Список иконок игроков
        protected List<string> PlayerNames = new List<string>(); // Список имен игроков
        protected List<Color> PlayerColors = new();

        protected virtual void OnEnable()
        {
            addPlayerButton.GetComponent<Button>().onClick.AddListener(OpenModelWindow);
        }

        protected virtual void OnDisable()
        {
            addPlayerButton.GetComponent<Button>().onClick.RemoveListener(OpenModelWindow);
        }

        protected virtual void OnPlayerAdded(string playerName)
        {
        }

        protected virtual void OnPlayerRemoved(int playerIndex)
        {
        }

        void OpenModal()
        {
            //modalPanel.SetActive(true);
            playerNameInputField.text = ""; // Очищаем поле ввода
        }

        void CloseModal()
        {
            //modalPanel.SetActive(false);
        }

        void OpenModelWindow()
        {
            // CanvasGroup canvasGroup = modalPanel.GetComponent<CanvasGroup>();
            // canvasGroup.alpha = 1;
            // canvasGroup.interactable = true;
            // canvasGroup.blocksRaycasts = true;
            // modalPanel.transform.SetAsLastSibling();
            
            modalPanel.SetActive(true);

            GameObject InputPlayer = modalPanel.transform.Find("InputPlayer").gameObject;
            GameObject AddPlayerButton = modalPanel.transform.Find("AddPlayerButton").gameObject;
            GameObject CloseButton = modalPanel.transform.Find("CloseButton").gameObject;

            AddPlayerButton.GetComponent<Button>().onClick.AddListener(OnAddPlayer);
            CloseButton.GetComponent<Button>().onClick.AddListener(OnClose);
        }

        void OnAddPlayer()
        {
            TMPro.TMP_InputField InputPlayer = modalPanel.transform.Find("InputPlayer").gameObject
                .GetComponent<TMPro.TMP_InputField>();
            string playerName = InputPlayer.text.Trim();

            if (string.IsNullOrEmpty(playerName))
            {
                Debug.LogWarning("Имя игрока не может быть пустым.");
                return;
            }

            if (PlayerNames.Contains(playerName))
            {
                Debug.LogWarning("Игрок с таким именем уже существует.");
                return;
            }

            PlayerNames.Add(playerName);
            CreatePlayerIcon(playerName);

            InputPlayer.text = "";
            UpdatePlayerPositions();

            OnPlayerAdded(playerName);


            // CanvasGroup canvasGroup = modalPanel.GetComponent<CanvasGroup>();
            // canvasGroup.alpha = 0;
            // canvasGroup.interactable = false;
            // canvasGroup.blocksRaycasts = false;
            modalPanel.SetActive(false);

            GameObject AddPlayerButton = modalPanel.transform.Find("AddPlayerButton").gameObject;
            GameObject CloseButton = modalPanel.transform.Find("CloseButton").gameObject;

            AddPlayerButton.GetComponent<Button>().onClick.RemoveAllListeners();
            CloseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        void OnClose()
        {
            // CanvasGroup canvasGroup = modalPanel.GetComponent<CanvasGroup>();
            // canvasGroup.alpha = 0;
            // canvasGroup.interactable = false;
            // canvasGroup.blocksRaycasts = false;
            modalPanel.SetActive(false);

            GameObject InputPlayer = modalPanel.transform.Find("InputPlayer").gameObject;
            GameObject AddPlayerButton = modalPanel.transform.Find("AddPlayerButton").gameObject;
            GameObject CloseButton = modalPanel.transform.Find("CloseButton").gameObject;

            AddPlayerButton.GetComponent<Button>().onClick.RemoveAllListeners();
            CloseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        public void CreatePlayerIcon(string playerName)
        {
            GameObject newIcon = Instantiate(playerIconPrefab, this.transform);
            if (PlayerColors.Count > 0)
            {
                var image = newIcon.GetComponent<Image>();
                image.color = PlayerColors[PlayerNames.Count];
            }

            TMP_Text iconText = newIcon.GetComponentInChildren<TMP_Text>(); // Получаем TMP_Text
            if (iconText != null)
            {
                iconText.text = playerName;
            }
            else
            {
                Debug.LogError("Не найден компонент TMP_Text в префабе иконки игрока!");
            }

            newIcon.transform.localScale = Vector3.one * iconScale; // Устанавливаем масштаб
            newIcon.AddComponent<PlayerIconDragHandler>(); // Добавляем скрипт для перетаскивания

            PlayerIconDragHandler dragHandler = newIcon.GetComponent<PlayerIconDragHandler>();
            dragHandler.playerSelectionWidget = this; // Передаем ссылку на виджет

            playerIcons.Add(newIcon);
        }

        protected void UpdatePlayerPositions()
        {
            // Расположение иконок по кругу
            float angleStep = 360f / playerIcons.Count;
            for (int i = 0; i < playerIcons.Count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                var rect = gameObject.GetComponent<RectTransform>().rect;
                float coeff = 0.35f;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * rect.width * coeff, Mathf.Sin(angle) * rect.height * coeff,
                    0);
                playerIcons[i].transform.localPosition = pos;
            }
        }

        public void SwapPlayerPositions(GameObject icon1, GameObject icon2)
        {
            // Меняем местами позиции иконок в списке
            int index1 = playerIcons.IndexOf(icon1);
            int index2 = playerIcons.IndexOf(icon2);

            if (index1 != -1 && index2 != -1)
            {
                GameObject temp = playerIcons[index1];
                playerIcons[index1] = playerIcons[index2];
                playerIcons[index2] = temp;
                var t = icon1.GetComponent<PlayerIconDragHandler>().startPosition;
                icon1.GetComponent<PlayerIconDragHandler>().startPosition =
                    icon2.GetComponent<PlayerIconDragHandler>().startPosition;
                icon2.GetComponent<PlayerIconDragHandler>().startPosition = t;

                UpdatePlayerPositions(); // Обновляем позиции на экране
            }
            else
            {
                Debug.LogError("Одна из иконок не найдена в списке!");
            }
        }


        public void RemovePlayer(GameObject icon)
        {
            int index = playerIcons.IndexOf(icon);

            if (index != -1)
            {
                string playerName = icon.GetComponentInChildren<TMP_Text>().text; // Получаем имя из иконки
                PlayerNames.Remove(playerName); // Удаляем имя из списка имен

                playerIcons.RemoveAt(index);
                
                Destroy(icon);
                UpdatePlayerPositions();

                OnPlayerRemoved(index);
            }
        }
    }
}