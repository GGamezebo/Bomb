using System;
using System.Collections.Generic;

namespace Lib
{
    public class EventManager
    {
        private Dictionary<string, Delegate> _eventDictionary = new();

        public void Add(string eventName, Delegate eventHandler)
        {
            if (!_eventDictionary.ContainsKey(eventName))
            {
                _eventDictionary[eventName] = eventHandler;
            }
            else
            {
                _eventDictionary[eventName] = Delegate.Combine(_eventDictionary[eventName], eventHandler);
            }
        }

        public void Remove(string eventName, Delegate eventHandler)
        {
            if (_eventDictionary.ContainsKey(eventName))
            {
                var currentDelegate = _eventDictionary[eventName];
                currentDelegate = Delegate.Remove(currentDelegate, eventHandler);
                if (currentDelegate == null)
                {
                    _eventDictionary.Remove(eventName);
                }
                else
                {
                    _eventDictionary[eventName] = currentDelegate;
                }
            }
        }

        public void Call(string eventName, params object[] args)
        {
            if (_eventDictionary.ContainsKey(eventName))
            {
                _eventDictionary[eventName].DynamicInvoke(args);
            }
        }

        public void Clear()
        {
            _eventDictionary.Clear();
        }
    }

    public class Event
    {
        private EventManager _eventManager;
        
        public Event(EventManager eventManager) => _eventManager = eventManager;
        ~Event() => _eventManager = null;

        public void Call(string eventName, params object[] args)
        {
            _eventManager.Call(eventName, args);
        }
    }
    
    public class EventListener
    {
        private EventManager _eventManager;
        private readonly Dictionary<string, List<Delegate>> _mapListeners = new();

        public EventListener(EventManager eventManager)
        {
            _eventManager = eventManager;
        }

        ~EventListener()
        {
            RemoveAllListeners();
            _eventManager = null;
        }

        public void Add(string eventName, Delegate eventHandler)
        {
            _eventManager.Add(eventName, eventHandler);

            if (!_mapListeners.ContainsKey(eventName))
            {
                _mapListeners[eventName] = new List<Delegate>();
            }

            List<Delegate> callbacks = _mapListeners[eventName];
            if (!callbacks.Contains(eventHandler))
            {
                callbacks.Add(eventHandler);
            }
        }

        public void Remove(string eventName, Delegate eventHandler)
        {
            if (_mapListeners.TryGetValue(eventName, out List<Delegate> callbacks))
            {
                if (callbacks.Contains(eventHandler))
                {
                    callbacks.Remove(eventHandler);
                    if (callbacks.Count == 0)
                    {
                        _mapListeners.Remove(eventName);
                    }
                }
            }
        }

        public void RemoveAllListeners()
        {
            foreach (var mapListener in _mapListeners)
            {
                foreach (var @delegate in mapListener.Value)
                {
                    _eventManager.Remove(mapListener.Key, @delegate);
                }
            }

            _mapListeners.Clear();
        }
    }
}