using System;
using System.Collections.Generic;
using Lib.Unity.Serialization;
using ScriptableObjects;
using UnityEngine;

namespace Account
{
    [Serializable]
    public class AccountPersistentData
    {
        public List<string> playerNames = new ();
    }

    public class AccountPersistentDataComponent : SerializablePersistentDataComponent<AccountPersistentData>
    {
        [Tooltip("Static global game settings")]
        [SerializeField] private GameSettings gameSettings;
        
#if UNITY_EDITOR
        protected override void OnPDataLoaded()
        {
            data.playerNames = gameSettings.devPlayerNames;
        }
#endif
    }
}