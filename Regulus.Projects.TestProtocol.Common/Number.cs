using Regulus.Remote;

namespace Regulus.Projects.TestProtocol.Common
{
    public class Number : INumber
    {
        public readonly Property<int> Value;

        public Number(int val)
        {
            Value = new Property<int>(val);
        }
        Property<int> INumber.Value => Value;
    }
}
