using Regulus.Serialization;
using System;
using System.Collections.Generic;



namespace Regulus.Remote
{
    public class AutoRelease<TKey,TValue> where TValue : class
    {
        private readonly Dictionary<TKey, WeakReference<TValue>> _Exists;

        private readonly IOpCodeExchangeable _Requester;
        private readonly IInternalSerializable _Serializer;

        public AutoRelease()
        {
            
            _Exists = new Dictionary<TKey, WeakReference<TValue>>();
        }


        public void Push(TKey key , TValue value)
        {
            
            WeakReference<TValue> instance;
            lock (_Exists)
            {
                if (_Exists.TryGetValue(key, out instance) == false)
                {
                    _Exists.Add(key, new WeakReference<TValue>(value));
                }
            }
            
        }


        public IEnumerable<TKey> NoExist()
        {
            List<TKey> ids = new List<TKey>();

            lock (_Exists)
            {
                foreach (KeyValuePair<TKey, WeakReference<TValue>> e in _Exists)
                {
                    TValue target;                    
                    if (!e.Value.TryGetTarget(out target))
                    {
                        ids.Add(e.Key);
                    }
                }
            }
            

            foreach (TKey id in ids)
            {
                lock(_Exists)
                    _Exists.Remove(id);
                yield return id;
            }
        }
    }
}
