namespace Regulus.Network.RUDP
{
    public class BufferDispenser
    {
        
        
        private readonly int _packageSize;

        private uint _Serial;

        public BufferDispenser(int package_size)
        {
            
            _packageSize = package_size;
        }

        public MessagePackage[] Packing(byte[] buffer, uint ack , uint ack_bits)
        {            
            var count = buffer.Length / _packageSize + 1;
            var packages = new MessagePackage[count];
            var ackBits = ack_bits;

            var buffserSize = buffer.Length;
            for (int i = count - 1; i >= 0; i--)
            {
                var begin = _packageSize * i;
                var writeSize = buffserSize - begin;
                var data = new byte[writeSize];
                
                for (int iDataIndex = 0; iDataIndex < writeSize; iDataIndex++)
                {
                    data[iDataIndex] = buffer[begin + iDataIndex];
                }
                buffserSize -= writeSize;

                var package = new MessagePackage();                
                package.Serial = _Serial + (uint)i;
                package.Ack = ack;
                package.AckBits = ackBits;
                package.Data = data;
                packages[i] = package;
            }
            _Serial += (uint)count;
            return packages;
        }
    }
}