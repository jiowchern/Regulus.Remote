using Regulus.Remote;

namespace Regulus.Projects.TestProtocol.Common
{
    public interface ISample
    {
        

        Regulus.Remote.Value<int> Add(int num1,int num2);

        
        event System.Action<int> IntsEvent;

        INotifier<INumber> Numbers { get; }

        Regulus.Remote.Value<bool> RemoveNumber(int val);
    }
}
