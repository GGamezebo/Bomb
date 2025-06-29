using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Account
{
    
    [Serializable]
    public class PlayerInfo
    {
        public string name;
        public int presetId;
        
        public PlayerInfo(string name, int presetId)
        {
            this.name = name;
            this.presetId = presetId;
        }
    }
    
    [Serializable]
    public class AccountPersistentData
    {
        public List<PlayerInfo> players = new ();
    }
}