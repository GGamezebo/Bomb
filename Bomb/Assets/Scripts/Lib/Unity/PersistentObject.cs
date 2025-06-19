using System.Collections.Generic;
using UnityEngine;

namespace Lib.Unity
{
    public class PersistentObject : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Unique identifier (leave empty for auto-generated)")]
        [SerializeField] private string persistentID = "";
        
        [Tooltip("Should duplicate objects be destroyed when new scene loads?")]
        [SerializeField] private bool destroyDuplicates = true;
        
        private static HashSet<string> _existingIDs =  new HashSet<string>();

        private void Awake()
        {
            InitializePersistentObject();
        }
        
        private void InitializePersistentObject()
        {
            if (string.IsNullOrEmpty(persistentID))
            {
                persistentID = System.Guid.NewGuid().ToString();
            }
            
            if (_existingIDs.Contains(persistentID))
            {
                if (destroyDuplicates)
                {
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                _existingIDs.Add(persistentID);
            }
            
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(persistentID))
            {
                _existingIDs.Remove(persistentID);
            }
        }
        
        public void DestroyPersistentObject()
        {
            if (!string.IsNullOrEmpty(persistentID))
            {
                _existingIDs.Remove(persistentID);
            }
            Destroy(gameObject);
        }
    }
}