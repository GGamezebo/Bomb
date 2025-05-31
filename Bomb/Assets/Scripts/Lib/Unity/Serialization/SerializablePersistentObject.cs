using UnityEngine;
using UnityEngine.Serialization;

namespace Lib.Unity.Serialization
{
    public class SerializablePersistentObject<T> : PersistentObject where T : new()
    {
        [Header("Serialization")]
        [SerializeField] private string saveFileName = "persistentObjects.json";
    
        public T data;

        protected override void InitializePersistentObject()
        {
            base.InitializePersistentObject();
            
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

        protected virtual void OnDisable()
        {
            Save();
        }

        public void Save()
        {
            if (data == null)
            {
                data = new T();
            }
            OnSaveState();
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
            OnLoadState();
        }

        protected virtual void OnSaveState() { }
        protected virtual void OnLoadState() { }
    }
}