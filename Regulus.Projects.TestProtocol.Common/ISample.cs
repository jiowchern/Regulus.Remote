using Regulus.Remote;

namespace Regulus.Projects.TestProtocol.Common
{
    public interface ISample
    {

        Regulus.Remote.Property<int> LastValue { get; }
        Regulus.Remote.Value<int> Add(int num1,int num2);

        
        event System.Action<int> IntsEvent;
        

        Regulus.Remote.Value<bool> RemoveNumber(int val);
    }
}
