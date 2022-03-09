using System;

namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    public class MethodTester : IMethodable
    {
        Value<int> IMethodable.GetValue0(int _1, string _2, float _3, double _4, decimal _5, Guid _6)
        {
            return 0;
        }

        Value<int> IMethodable1.GetValue1()
        {
            return 1;
        }

        Value<int> IMethodable2.GetValue2()
        {
            return 2;
        }

        int IMethodable.NotSupported()
        {
            return 0;
        }
    }
}
