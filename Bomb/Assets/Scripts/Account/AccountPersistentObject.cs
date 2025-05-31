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

    public class AccountPersistentObject : SerializablePersistentObject<AccountPersistentData>
    {
        public GameSettings gameSettings;
        
#if UNITY_EDITOR
        protected override void OnLoadState()
        {
            data.playerNames = gameSettings.devPlayerNames;
        }
#endif
    }
}