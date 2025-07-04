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
        [SerializeField] protected GameObject playerSelection;
        [SerializeField] protected List<GameObject> colors;
        
        private PlayerSelectionWidget _playerSelectionWidget;
        protected GameObject SelectedColorItem;

        protected virtual string PlayerImageBasePath => "";


        protected virtual void Awake()
        {
            _playerSelectionWidget = playerSelection.GetComponent<PlayerSelectionWidget>();
        }
        
        protected virtual void OnEnable()
        {
            playerName.text = "";
            playerName.ActivateInputField();
        }

        protected virtual void OnDisable()
        {
            
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