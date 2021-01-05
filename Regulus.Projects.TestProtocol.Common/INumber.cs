using Regulus.Remote;

namespace Regulus.Projects.TestProtocol.Common
{
    public interface INumber
    {
        Property<int> Value { get; }
    }
}
