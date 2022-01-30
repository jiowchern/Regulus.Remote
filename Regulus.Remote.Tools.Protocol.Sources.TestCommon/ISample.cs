using Regulus.Remote;

namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    public struct StructA
    {
        public System.Guid Field1;
        public int Num1;
    }

    public struct ClassA
    {
        public int[] Num1;
    }


    public interface ISample
    {

        Regulus.Remote.Property<int> LastValue { get; }
        Regulus.Remote.Value<int> Add(int num1,int num2);

        
        event System.Action<int> IntsEvent;
        
        Regulus.Remote.Notifier<INumber> Numbers { get; }
        Regulus.Remote.Value<bool> RemoveNumber(int val);
    }
}
