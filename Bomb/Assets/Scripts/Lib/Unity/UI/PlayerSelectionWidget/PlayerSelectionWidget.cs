using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lib.Unity.UI.PlayerSelectionWidget
{
    public class PlayerSelectionWidget : MonoBehaviour
    {
        [SerializeField] public GameObject addPlayerButton;
        [SerializeField] public GameObject playerIconPrefab;
        public List<GameObject> playerIcons = new ();
        
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void OnPlayerAdded(string playerName, int presetId) { }
        protected virtual void OnPlayerRemoved(int playerIndex) { }
        
        protected virtual void Awake() {}

        public void AddPlayer(string playerName, int presetId)
        {
            CreatePlayerIcon(playerName, presetId);
            UpdatePlayerPositions();
            OnPlayerAdded(playerName, presetId);
        }
        
        public void CreatePlayerIcon(string playerName, int presetId)
        {
            GameObject newIcon = Instantiate(playerIconPrefab, this.transform);
            var slimeImage = newIcon.transform.Find("CircleWithOutline/Slime").GetComponent<Image>();
            var playerNameText = newIcon.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
            slimeImage.sprite = Resources.Load<Sprite>( $"Slimes/{presetId}");
            playerNameText.text = playerName;
            
            AddComponent(newIcon);
            PlayerIconDragHandler dragHandler = newIcon.GetComponent<PlayerIconDragHandler>();
            dragHandler.playerSelectionWidget = this;
            
            playerIcons.Add(newIcon);
        }

        protected virtual void AddComponent(GameObject newIcon)
        {
            newIcon.AddComponent<PlayerIconDragHandler>();
        }

        protected void UpdatePlayerPositions()
        {
         
            float angleStep = 360f / playerIcons.Count;
            var table = gameObject.transform.Find("Table").gameObject;
            for (int i = 0; i < playerIcons.Count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                var rect = table.GetComponent<RectTransform>().rect;
                float coeff = 0.5f;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * rect.width * coeff, Mathf.Sin(angle) * rect.height * coeff, 0);
                playerIcons[i].transform.localPosition = pos;
                playerIcons[i].GetComponent<PlayerIconDragHandler>().ResetPosition(pos);
            }

            // var places = gameObject.transform.Find("Table/Places").gameObject;
            // var scheme = places.transform.Find($"{playerIcons.Count}").gameObject;
            // for (int i = 0; i < playerIcons.Count; i++)
            // {
            //     var placeTransform = scheme.transform.Find($"{i}");
            //     playerIcons[i].transform.position = placeTransform.position;
            //     playerIcons[i].GetComponent<PlayerIconDragHandler>().ResetPosition(placeTransform.position);
            // }
        }

        public void SwapPlayerPositions(GameObject icon1, GameObject icon2)
        {
            // Меняем местами позиции иконок в списке
            int index1 = playerIcons.IndexOf(icon1);
            int index2 = playerIcons.IndexOf(icon2);

            if (index1 != -1 && index2 != -1)
            {
                (playerIcons[index1], playerIcons[index2]) = (playerIcons[index2], playerIcons[index1]);
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
                playerIcons.RemoveAt(index);
                OnPlayerRemoved(index);
                Destroy(icon);
                UpdatePlayerPositions();
            }
        }
    }
}