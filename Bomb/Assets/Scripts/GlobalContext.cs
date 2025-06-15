using System;
using Account;
using Lib;
using UnityEngine;
using Event = Lib.Event;

public class GlobalContext : MonoBehaviour
{
    private EventManager _eventManager;
    
    [Tooltip("Account persistent data")]
    [SerializeField] public AccountPersistentDataComponent accountData;

    public GameLogic.Storage Storage;
    
    void Awake()
    {
        _eventManager = new EventManager();
        accountData = GetComponent<AccountPersistentDataComponent>();
        Storage = GetComponent<GameLogic.Storage>();
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
        return accountData.data;
    }
}

