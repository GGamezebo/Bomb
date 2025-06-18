using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Account
{
    
    [Serializable]
    public class PlayerInfo
    {
        public string name;
        public int colorPresetId;
        
        public PlayerInfo(string name, int colorPresetId)
        {
            this.name = name;
            this.colorPresetId = colorPresetId;
        }
    }
    
    [Serializable]
    public class AccountPersistentData
    {
        public List<PlayerInfo> players = new ();
    }
}