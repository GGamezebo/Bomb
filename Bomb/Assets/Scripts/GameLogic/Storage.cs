using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;


namespace GameLogic
{
    public class PlayerPreset
    {
        public readonly Sprite ColorTag;
        
        public PlayerPreset(Sprite colorTag)
        {
            ColorTag = colorTag;
        }

        public override string ToString()
        {
            return $"PlayerPreset {{ ColorTag = {ColorTag}}}";
        }
    }
    
    public class Storage : MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;
        private Dictionary<int, PlayerPreset> _storage = new ();
        private readonly HashSet<int> _locks = new ();
        
        
        private void Awake()
        {
            for (int i = 0; i < gameSettings.colorIcons.Count; i++) // Если это массив или List
            {
                _storage.Add(i, new PlayerPreset(gameSettings.colorIcons[i]));
            }
        }

        public bool IsLock(int presetId)
        {
            return _locks.Contains(presetId);
        }

        PlayerPreset Take(int presetId)
        {
            _locks.Add(presetId);
            return _storage[presetId];
        }

        void Free(int presetId)
        {
            _locks.Remove(presetId);
        }
    }
}