using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;
using Common;
using System;
using Account;


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
        
        
        protected override void Awake()
        {
            for (int i = 0; i < gameSettings.colorIcons.Count; i++) // Если это массив или List
            {
                _storage.Add(i, new PlayerPreset(gameSettings.colorIcons[i]));
            }
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventListener.Add(Events.EvPlayerAdded, new Action<PlayerInfo>(OnPlayerAdded));
            EventListener.Add(Events.EvPlayerRemoved, new Action<PlayerInfo>(OnPlayerRemoved));
            EventListener.Add(Events.EvPlayerModified, new Action<PlayerInfo>(OnPlayerModified));
        }

        private void Start()
        {
            UpdateAllStorage();
        }

        public bool isHold(int presetId)
        {
            return _locks.Contains(presetId);
        }
        
        public PlayerPreset Take(int presetId)
        {
            _locks.Add(presetId);
            return _storage[presetId];
        }

        public void Free(int presetId)
        {
            _locks.Remove(presetId);
        }

        private void OnPlayerAdded(PlayerInfo playerName)
        {
            UpdateAllStorage();
        }

        private void OnPlayerRemoved(PlayerInfo playerName)
        {
            UpdateAllStorage();
        }
        
        private void OnPlayerModified(PlayerInfo playerName)
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