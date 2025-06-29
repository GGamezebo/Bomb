using System.IO;
using UnityEngine;

namespace Lib.Unity.Serialization
{
    public static class SerializationManager
    {

        private static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
    
        public static void Save<T>(T data, string fileName)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetPath(fileName), json);
        }

        public static T Load<T>(string fileName) where T : new()
        {
            string fullPath = GetPath(fileName);
            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                return JsonUtility.FromJson<T>(json);
            }
        
            return new T();
        }

        public static bool IsExists(string fileName)
        {
            return File.Exists(GetPath(fileName));
        }

        public static void DeleteFile(string fileName)
        {
            if (IsExists(fileName))
            {
                File.Delete(GetPath(fileName));
            }
        }
    }
}