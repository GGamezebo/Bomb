using UnityEngine;

namespace Common
{
    public class GameObserverMonoBehaviour : MonoBehaviour
    {
        protected Lib.EventListener EventListener;

        protected virtual void Awake()
        {
            var globalContext = FindFirstObjectByType<GlobalContext>();
            EventListener = globalContext.MakeEventListener();
        }

        protected virtual void OnDestroy()
        {
            EventListener = null;
        }

        protected virtual void OnEnable()
        {
            Subscribe();
        }

        protected virtual void OnDisable()
        {
            EventListener.RemoveAllListeners();
        }

        protected virtual void Subscribe() { }
    }
}