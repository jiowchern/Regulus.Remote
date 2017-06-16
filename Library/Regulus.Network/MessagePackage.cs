namespace Regulus.Network.RUDP
{
    
    public struct MessagePackage 
    {
        public uint Serial;
        public uint Ack;
        public uint AckBits;
        public byte[] Data;

        public static int GetHeadSize()
        {            
            return 12+4+4;
        }

        public static int GetSize()
        {
            return Peer.PackageSize;
        }
        public static int GetBodySize()
        {
            return GetSize() - GetHeadSize();
        }
    }
}