using Regulus.Remote;
using Regulus.Remote.Tools.Protocol.Sources.TestCommon;
using System;

namespace Regulus.Integration.Tests
{
    namespace SimulateReals
    {
        namespace Server
        {
            class User : Regulus.Remote.Tools.Protocol.Sources.TestCommon.IMethodable
            {
                public User(IBinder binder)
                {
                    binder.Bind<Regulus.Remote.Tools.Protocol.Sources.TestCommon.IMethodable>(this);
                }

                Value<int[]> IMethodable.GetValue0(int _1, string _2, float _3, double _4, decimal _5, Guid _6)
                {
                    throw new NotImplementedException();
                }

                Value<int> IMethodable1.GetValue1()
                {
                    throw new NotImplementedException();
                }

                Value<int> IMethodable2.GetValue2()
                {
                    throw new NotImplementedException();
                }

                Value<IMethodable> IMethodable.GetValueSelf()
                {
                    throw new NotImplementedException();
                }

                int IMethodable.NotSupported()
                {
                    throw new NotImplementedException();
                }

                Value<HelloReply> IMethodable2.SayHello(HelloRequest request)
                {
                    return new Value<HelloReply>(new HelloReply());
                }
            }
        }
    }
    
}