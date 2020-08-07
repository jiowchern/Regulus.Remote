namespace Regulus.Network
{
    public interface ISerialProvider
    {
        uint[] AllocateSerial(int Count);
        uint Ack { get; }
        uint AckBits { get; }
    }
}