using System.Collections.Generic;
using UnityEngine;

namespace Lib.Unity
{
    /// <summary>
    /// GameObject that persists between scene loads with duplicate prevention
    /// </summary>
    public class PersistentObject : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Unique identifier (leave empty for auto-generated)")]
        [SerializeField] private string persistentID = "";
        
        [Tooltip("Should duplicate objects be destroyed when new scene loads?")]
        [SerializeField] private bool destroyDuplicates = true;

        // Track all existing persistent IDs
        private static HashSet<string> _existingIDs =  new HashSet<string>();

        private void Awake()
        {
            InitializePersistentObject();
        }

        /// <summary>
        /// Handles object persistence initialization
        /// </summary>
        protected virtual void InitializePersistentObject()
        {
            // Generate ID if empty
            if (string.IsNullOrEmpty(persistentID))
            {
                persistentID = System.Guid.NewGuid().ToString();
            }

            // Check for duplicates
            if (_existingIDs.Contains(persistentID))
            {
                if (destroyDuplicates)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                _existingIDs.Add(persistentID);
            }

            // Make object persistent
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Clean up ID when object is destroyed
        /// </summary>
        private void OnDestroy()
        {
            if (!string.IsNullOrEmpty(persistentID))
            {
                _existingIDs.Remove(persistentID);
            }
        }

        /// <summary>
        /// Manually destroy this persistent object
        /// </summary>
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