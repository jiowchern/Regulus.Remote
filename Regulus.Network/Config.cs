namespace Regulus.Network
{
    public class Config
    {

        public Config(int Mtu)
        {
            this.Mtu = Mtu;
            PackageSize = this.Mtu - Pv4HeadSize - UdpHeadSize;
        }

        public static readonly float Timeout = 300;
        public const int Pv4HeadSize = 20;
        public const int UdpHeadSize = 8;
        public readonly int Mtu;
        public readonly int PackageSize;


        public static readonly Config Default = new Config(Mtu: 1500);
    }
}
