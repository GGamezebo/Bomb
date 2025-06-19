using System;
using Account;
using Lib;
using UnityEngine;
using ScriptableObjects;
using Event = Lib.Event;

public class GlobalContext : MonoBehaviour
{
    private EventManager _eventManager;
    
    [Tooltip("Static global game settings")]
    [SerializeField] public GameSettings gameSettings;
    
    [Tooltip("Account persistent data")]
    [SerializeField] public AccountPersistentDataComponent accountDataComponent;

    public GameLogic.PlayerPresetStorage playerPresetStorage;
    
    void Awake()
    {
        _eventManager = new EventManager();
        accountDataComponent = GetComponent<AccountPersistentDataComponent>();
        playerPresetStorage = GetComponent<GameLogic.PlayerPresetStorage>();
    }

    public EventListener MakeEventListener()
    {
        return new EventListener(this._eventManager);
    }
    
    public Event MakeEvent()
    {
        return new Event(this._eventManager);
    }   
    
    public AccountPersistentData PData()
    {
        return accountDataComponent.data;
    }
}

