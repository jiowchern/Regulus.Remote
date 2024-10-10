using System;
using System.Diagnostics;


namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon.Tests
{
    public class Entry<T> : IEntry, System.IDisposable
    {
        readonly T _Entry;
        public Entry(T entry)
        {
            _Entry = entry;
        }
        void IBinderProvider.RegisterClientBinder(IBinder binder)
        {
            binder.Bind(_Entry);
        }

        void IBinderProvider.UnregisterClientBinder(IBinder binder)
        {
            //binder.Unbind(_Entry);
        }
        void IDisposable.Dispose()
        {
        }

        void IEntry.Update()
        {
            
        }
    }
}