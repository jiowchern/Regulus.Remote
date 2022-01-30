using Regulus.Remote;

namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    
    public class TestC
    {
        public string F1;
    }
    public interface ITest
    {
        void M1(TestC a1,TestS a2);
    }
    public struct TestS
    {
        public string F1;
    }

    public interface INext
    {
        Remote.Value<bool> Next();
    }
    public interface INumber
    {
        Property<int> Value { get; }
    }
}
