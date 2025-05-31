using UnityEngine;

namespace Lib.Unity
{

    public class PersistentSingletonObject : MonoBehaviour
    {
        private static PersistentSingletonObject Instance { get; set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}