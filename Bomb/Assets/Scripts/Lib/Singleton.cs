using System;

namespace Lib
{
    public class Singleton<T> where T : class, new()
    {
        private Singleton() {}
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());
        
        public static T Instance => _instance.Value;
    }
}