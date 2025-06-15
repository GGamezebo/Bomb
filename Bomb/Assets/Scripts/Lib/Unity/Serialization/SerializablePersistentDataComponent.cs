using UnityEngine;


namespace Lib.Unity.Serialization
{
    public class SerializablePersistentDataComponent<T> : MonoBehaviour where T : new()
    {
        [Header("Serialization")]
        [SerializeField] private string saveFileName = "persistentObjects.json";
    
        public T data;

        private void Awake()
        {
            
// #if UNITY_ANDROID
//             if (Application.platform == RuntimePlatform.Android)
//             {
//                 if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageWrite))
//                 {
//                     // Запрос разрешения
//                     UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageWrite);
//                     // После этого разрешения, пользователь увидит диалог.
//                     // Результат можно отследить через PermisisonCallbacks
//                     // (сложнее, для начала просто запросите)
//                 }
//             }
// #endif
            
            Load();
        }

        private void OnDisable()
        {
            Save();
        }

        public void Save()
        {
            if (data == null)
            {
                data = new T();
            }
            OnPDataSave();
            SerializationManager.Save(data, saveFileName);
        }

        public void Load()
        {
            if (SerializationManager.IsExists(saveFileName))
            {
                data = SerializationManager.Load<T>(saveFileName);
            }
            else
            {
                data = new T();
            }
            OnPDataLoaded();
        }

        protected virtual void OnPDataSave() { }
        protected virtual void OnPDataLoaded() { }
    }
}