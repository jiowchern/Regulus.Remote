namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    public class MethodTester : IMethodable
    {
        Value<int> IMethodable1.GetValue1()
        {
            return 1;
        }

        Value<int> IMethodable2.GetValue2()
        {
            return 2;
        }
    }
}
