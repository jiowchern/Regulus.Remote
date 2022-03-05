using System;

namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon.Tests
{
    public class Entry<T> : IBinderProvider, System.IDisposable
    {
        readonly T _Entry;
        public Entry(T entry)
        {
            _Entry = entry;
        }
        void IBinderProvider.AssignBinder(IBinder binder)
        {
            binder.Bind(_Entry);
        }

        void IDisposable.Dispose()
        {
        }
    }
}