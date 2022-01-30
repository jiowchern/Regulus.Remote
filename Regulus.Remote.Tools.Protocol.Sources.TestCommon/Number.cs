using Regulus.Remote;

namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
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
