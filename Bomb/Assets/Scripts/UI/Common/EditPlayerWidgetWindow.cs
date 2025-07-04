using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;
using System.Linq;
using UnityEngine.UI;

namespace Common
{
    public class EditPlayerWidgetWindow : Lib.Unity.UI.PlayerSelectionWidget.EditPlayerWidgetWindow
    {
        [SerializeField] public GameObject globalContext;

        protected override string PlayerImageBasePath => "Slimes";
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            var playerPresetStorage = globalContext.GetComponent<GlobalContext>().playerPresetStorage;
            GameObject selectedColorItem = null;
            for (int i = 0; i < colors.Count; i++)
            {
                var colorItem = colors[i];
                var unavailable = colorItem.transform.Find("Unavailable").gameObject;
                unavailable.SetActive(playerPresetStorage.isHold(i));
                
                if (!unavailable.active)
                {
                    if (selectedColorItem == null)
                    {
                        selectedColorItem = colorItem;
                        SelectColorItem(colorItem);
                    }
                    
                    colorItem.AddComponent<Button>();
                }
            }

            UpdateOkButton();
            SortPlayerPresets();
        }
        
        private void SortPlayerPresets()
        {
            List<GameObject> sortedList = new List<GameObject>();
            var playerPresetStorage = globalContext.GetComponent<GlobalContext>().playerPresetStorage;

            for (int i = 0; i < colors.Count; i++)
            {
                if (!playerPresetStorage.isHold(i))
                {
                    sortedList.Add(colors[i]);
                }
            }
            
            for (int i = 0; i < colors.Count; i++)
            {
                if (playerPresetStorage.isHold(i))
                {
                    sortedList.Add(colors[i]);
                }
            }
            
            for (int i = 0; i < sortedList.Count; i++)
            {
                sortedList[i].transform.SetSiblingIndex(i);
            }
        }
    }
}