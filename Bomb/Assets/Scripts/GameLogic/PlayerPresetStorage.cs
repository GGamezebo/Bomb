using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;
using Common;
using System;


namespace GameLogic
{
    public class PlayerPreset
    {
        private readonly Sprite _colorTag;
        
        public PlayerPreset(Sprite colorTag)
        {
            _colorTag = colorTag;
        }

        public override string ToString()
        {
            return $"PlayerPreset {{ ColorTag = {_colorTag}}}";
        }
    }
    
    public class PlayerPresetStorage : GameObserverMonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;
        private readonly Dictionary<int, PlayerPreset> _storage = new ();
        private readonly HashSet<int> _locks = new ();
        
        
        private void Awake()
        {
            for (int i = 0; i < gameSettings.colorIcons.Count; i++) // Если это массив или List
            {
                _storage.Add(i, new PlayerPreset(gameSettings.colorIcons[i]));
            }
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            _eventListener.Add(Events.EvPlayerAdded, new Action<string>(OnPlayerAdded));
            _eventListener.Add(Events.EvPlayerRemoved, new Action<string>(OnPlayerRemoved));
        }

        private void Start()
        {
            UpdateAllStorage();
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

        private void OnPlayerAdded(string playerName)
        {
            UpdateAllStorage();
        }

        private void OnPlayerRemoved(string playerName)
        {
            UpdateAllStorage();
        }

        private void UpdateAllStorage()
        {
            _locks.Clear();
            var globalContext = gameObject.GetComponent<GlobalContext>();
            foreach (var playerInfo in globalContext.PData().players)
            {
                globalContext.playerPresetStorage.Take(playerInfo.presetId);
            }
        }
    }
}