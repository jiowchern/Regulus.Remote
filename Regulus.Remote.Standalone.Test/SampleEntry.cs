using Regulus.Projects.TestProtocol.Common;
using System;

namespace Regulus.Remote.Standalone.Test
{
    public class SampleEntry : IBinderProvider , System.IDisposable
    {
        public readonly Regulus.Projects.TestProtocol.Common.Sample Sample;

        readonly System.IDisposable _SampleDispose;

        public SampleEntry()
        {
            Sample = new Projects.TestProtocol.Common.Sample();
            _SampleDispose = Sample;
        }
        void IBinderProvider.AssignBinder(IBinder binder, object state)
        {
            binder.Bind<ISample>(Sample);
        }

        void IDisposable.Dispose()
        {
            _SampleDispose.Dispose();
        }
    }
}
