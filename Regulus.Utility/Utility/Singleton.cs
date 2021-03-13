using System;

namespace Regulus.Utility
{
    public sealed class Singleton<T> where T : new()
    {
        private static T _New()
        {
            return new T();
        }

        public static T Instance { get {
                return _Instance.Value;

            } }
        private Singleton()
        {
        }        
        private static readonly Lazy<T> _Instance = new Lazy<T>(_New,  System.Threading.LazyThreadSafetyMode.PublicationOnly);
    }
}
