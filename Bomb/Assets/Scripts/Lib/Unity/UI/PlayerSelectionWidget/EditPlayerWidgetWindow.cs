using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lib.Unity.UI.PlayerSelectionWidget
{
    public class EditPlayerWidgetWindow : MonoBehaviour
    {
        [SerializeField] public TMP_InputField playerName;
        [SerializeField] public Image playerImage;
        [SerializeField] public Button ok;
        [SerializeField] public Button apply;
        [SerializeField] protected GameObject playerSelection;
        [SerializeField] protected List<GameObject> colors;
        
        protected GameObject SelectedColorItem;
        private int _playerIndex = -1;
        protected int editablePresetId = -1;

        protected virtual string PlayerImageBasePath => "";


        protected virtual void Awake() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        public void OpenAddPlayerWindow()
        {
            ok.gameObject.SetActive(true);
            apply.gameObject.SetActive(false);
            editablePresetId = -1;
            OpenWidow("");
        }

        public void OpenEditWindow(int playerIndex, string _playerName, int presetId)
        {
            ok.gameObject.SetActive(false);
            apply.gameObject.SetActive(true);
            _playerIndex = playerIndex;
            editablePresetId = presetId;
            OpenWidow(_playerName);
            SelectColorItem(colors[presetId]);
        }

        private void OpenWidow(string _playerName)
        {
            gameObject.SetActive(true);
            playerName.text = _playerName;
            playerName.ActivateInputField();
        }
        
        public void OnColorClicked(BaseEventData eventData)
        {
            PointerEventData pointerData = (PointerEventData)eventData;
            GameObject clickedObject = pointerData.pointerPress;
            var unavailable = clickedObject.transform.Find("Unavailable").gameObject;
            if (!unavailable.active)
            {
                foreach (var color in colors)
                {
                    var childOutline = color.GetComponent<Outline>();
                    childOutline.enabled = false;
                }
                
                Outline outline = clickedObject.GetComponent<Outline>();
                outline.enabled = true;
                
                SelectColorItem(clickedObject);
            }
        }

        public void OnOkClicked()
        {
            gameObject.SetActive(false);

            var newPlayerName = playerName.text.Trim();
            var presetId = colors.IndexOf(SelectedColorItem);

            var selectionWidget = playerSelection.GetComponent<PlayerSelectionWidget>();
            selectionWidget.AddPlayer(newPlayerName, presetId);
        }       
        
        public void OnApplyClicked()
        {
            gameObject.SetActive(false);

            var newPlayerName = playerName.text.Trim();
            var presetId = colors.IndexOf(SelectedColorItem);

            var selectionWidget = playerSelection.GetComponent<PlayerSelectionWidget>();
            selectionWidget.ApplyPlayer(_playerIndex, newPlayerName, presetId);
        }

        public void OnCancelClicked()
        {
            gameObject.SetActive(false);
        }
        
        public void OnTextChanged(string text)
        {
            UpdateOkButton();
        }

        protected void SelectColorItem(GameObject colorItem)
        {
            if (SelectedColorItem != null)
            {
                var prevSelected = SelectedColorItem.transform.Find("Selected").gameObject;
                prevSelected.SetActive(false);
            }
            
            SelectedColorItem = colorItem;
            var selected = SelectedColorItem.transform.Find("Selected").gameObject;
            selected.SetActive(true);
            
            var presetId = colors.IndexOf(SelectedColorItem);
            playerImage.sprite = Resources.Load<Sprite>(PlayerImageBasePath + $"/{presetId}");
        }

        protected void UpdateOkButton()
        {
            ok.interactable = playerName.text.Length > 0;
        }
    }
}