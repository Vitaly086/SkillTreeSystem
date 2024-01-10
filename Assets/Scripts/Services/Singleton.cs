using System;

namespace Services
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> _instance = new(() => new T());

        public static T Instance => _instance.Value;
    }
}