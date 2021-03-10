using System;

namespace Regulus.Utility
{
    public sealed class Singleton<T> where T : new()
    {
        private static readonly Lazy<T> _Instance = new Lazy<T>(() => new T());
        public static T Instance { get { return _Instance.Value; } }
        private Singleton()
        {
        }
    }
}
