using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;
using Common;
using System;


namespace GameLogic
{
    public class PlayerPreset
    {
        private readonly Sprite ColorTag;
        
        public PlayerPreset(Sprite colorTag)
        {
            ColorTag = colorTag;
        }

        public override string ToString()
        {
            return $"PlayerPreset {{ ColorTag = {ColorTag}}}";
        }
    }
    
    public class PlayerPresetStorage : GameObserverMonoBehaviour
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

        protected void Subscribe()
        {
            base.Subscribe();
            _eventListener.Add(Events.EvPlayerAdded, new Action<string>(OnPlayerAdded));
            _eventListener.Add(Events.EvPlayerRemoved, new Action<string>(OnPlayerRemoved));
        }

        private void Start()
        {
            base.Start();
            updateAllStorage();
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
            updateAllStorage();
        }

        private void OnPlayerRemoved(string playerName)
        {
            updateAllStorage();
        }

        private void updateAllStorage()
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