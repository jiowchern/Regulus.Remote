using Regulus.Projects.TestProtocol.Common;

namespace Regulus.Remote.Standalone.Test
{
    public class SampleEntry : IBinderProvider 
    {
        public readonly Regulus.Projects.TestProtocol.Common.Sample Sample;

        public SampleEntry()
        {
            Sample = new Projects.TestProtocol.Common.Sample();
        }
        void IBinderProvider.AssignBinder(IBinder binder, object state)
        {
            binder.Bind<ISample>(Sample);
        }

        
    }
}
