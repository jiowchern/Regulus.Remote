namespace Regulus.Network.RUDP
{
    public interface ISerialProvider
    {
        uint[] AllocateSerial(int count);
        uint Ack { get;  }
        uint AckBits { get;  }
    }
}