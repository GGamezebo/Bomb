using System.IO;
using UnityEngine;

namespace Lib.Unity.Serialization
{
    public static class SerializationManager
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "saves");
    
        public static void Save<T>(T data, string fileName)
        {
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
        
            string json = JsonUtility.ToJson(data, true);
            string fullPath = Path.Combine(SavePath, fileName);
            File.WriteAllText(fullPath, json);
        }

        public static T Load<T>(string fileName) where T : new()
        {
            string fullPath = Path.Combine(SavePath, fileName);
            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                return JsonUtility.FromJson<T>(json);
            }
        
            return new T();
        }

        public static bool IsExists(string fileName)
        {
            string fullPath = Path.Combine(SavePath, fileName);
            return File.Exists(fullPath);
        }

        public static void DeleteFile(string fileName)
        {
            if (IsExists(fileName))
            {
                string fullPath = Path.Combine(SavePath, fileName);
                File.Delete(fullPath);
            }
        }
    }
}