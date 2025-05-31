using Account;
using Lib;
using UnityEngine;
using UnityEngine.Serialization;
using Event = Lib.Event;

public class GlobalContext : MonoBehaviour
{
    private EventManager _eventManager;
    public AccountPersistentObject AccountData;
    
    void Awake()
    {
        _eventManager = new EventManager();
        AccountData = GetComponent<AccountPersistentObject>();
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
        return AccountData.data;
    }
}

