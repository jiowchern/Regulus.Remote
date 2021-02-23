using Regulus.Remote;

namespace Regulus.Projects.TestProtocol.Common
{
    public interface INext
    {
        Remote.Value<bool> Next();
    }
    public interface INumber
    {
        Property<int> Value { get; }
    }
}
