using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Lib.Unity.UI.PlayerSelectionWidget
{
    public class PlayerSelectionWidget : MonoBehaviour
    {
        [SerializeField] public GameObject addPlayerButton;
        [SerializeField] public GameObject editPlayerWindow;
        [SerializeField] public GameObject playerIconPrefab;
        [SerializeField] GameObject chairPrefab;
        public List<GameObject> playerIcons = new ();
        private List<GameObject> _chairs = new ();
        
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void OnPlayerAdded(string playerName, int presetId) { }
        protected virtual void OnPlayerModified(int playerIndex, string playerName, int presetId) { }
        protected virtual void OnPlayerRemoved(int playerIndex) { }
        protected virtual void OnSwapPlayerPositions(int index1, int index2) { }

        protected virtual void Awake() {}

        public void Clear()
        {
            foreach (var icon in playerIcons.ToList())
            {
                RemovePlayerIcon(icon);
            }
            UpdatePlayerPositions();
        }
        
        public virtual bool IsInteractable()
        {
            return true;
        }

        public void OpenEditWindow(int index, string playerName, int presetId)
        {
            var scriptComponent = editPlayerWindow.GetComponent<EditPlayerWidgetWindow>();
            scriptComponent.OpenEditWindow(index, playerName, presetId);
        }
        
        public void AddPlayer(string playerName, int presetId)
        {
            CreatePlayerIcon(playerName, presetId);
            UpdatePlayerPositions();
            OnPlayerAdded(playerName, presetId);
        }        
        
        public void ApplyPlayer(int playerIndex, string newPlayerName, int presetId)
        {
            SetPlayerParams(playerIcons[playerIndex], newPlayerName, presetId);
            OnPlayerModified(playerIndex, newPlayerName, presetId);
        }
        
        public void CreatePlayerIcon(string playerName, int presetId)
        {
            GameObject newIcon = Instantiate(playerIconPrefab, this.transform);
            SetPlayerParams(newIcon, playerName, presetId);
            
            AddComponent(newIcon);
            PlayerIconDragHandler dragHandler = newIcon.GetComponent<PlayerIconDragHandler>();
            dragHandler.playerSelectionWidget = this;
            
            playerIcons.Add(newIcon);
            _chairs.Add(Instantiate(chairPrefab, this.transform));
        }

        private void SetPlayerParams(GameObject playerIcon, string playerName, int presetId)
        {
            var slimeImage = playerIcon.transform.Find("CircleWithOutline/Slime").GetComponent<Image>();
            var playerNameText = playerIcon.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
            playerNameText.text = playerName;
            slimeImage.sprite = Resources.Load<Sprite>( $"Slimes/{presetId}");
            
        }

        protected virtual void AddComponent(GameObject newIcon)
        {
            newIcon.AddComponent<PlayerIconDragHandler>();
        }

        protected void UpdatePlayerPositions()
        {
         
            float angleStep = 360f / playerIcons.Count;
            var table = gameObject.transform.Find("Table").gameObject;
            var tableRectTransform = table.GetComponent<RectTransform>();
            var rect = tableRectTransform.rect;
            var center = tableRectTransform.position;
            Debug.Log(tableRectTransform.root.localScale.x);
            float coeff = 0.45f * tableRectTransform.root.localScale.x;
            for (int i = 0; i < playerIcons.Count; i++)
            {
                float angle = -i * angleStep * Mathf.Deg2Rad;
                Vector3 pos = center + new Vector3(Mathf.Cos(angle) * rect.width * coeff, Mathf.Sin(angle) * rect.height * coeff, 0);
                playerIcons[i].transform.position = pos;
                playerIcons[i].GetComponent<PlayerIconDragHandler>().ResetPosition(pos);
                
                _chairs[i].transform.position = pos;
                _chairs[i].transform.rotation = Quaternion.Euler(0, 0, i * angleStep - 90);
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

                OnSwapPlayerPositions(index1, index2);

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
                RemovePlayerIconByIndex(icon, index);
                UpdatePlayerPositions();
                OnPlayerRemoved(index);
            }
        }

        private void RemovePlayerIcon(GameObject icon)
        {
            int index = playerIcons.IndexOf(icon);
            if (index != -1)
            {
                RemovePlayerIconByIndex(icon, index);
            }
        }

        private void RemovePlayerIconByIndex(GameObject icon, int index)
        {
            var chair = _chairs[index];
            _chairs.RemoveAt(index);
            playerIcons.RemoveAt(index);
            Destroy(icon);
            Destroy(chair);
        }
    }
}