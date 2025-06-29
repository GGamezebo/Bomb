
using Lib.Unity.Serialization;
using ScriptableObjects;
using UnityEngine;

namespace Account
{

    public class AccountPersistentDataComponent : SerializablePersistentDataComponent<AccountPersistentData>
    {
        [Tooltip("Static global game settings")]
        [SerializeField] private GameSettings gameSettings;
        
#if UNITY_EDITOR
        protected override void OnPDataLoaded()
        {
            data.players = gameSettings.devPlayerNames;
        }
#endif
    }
}