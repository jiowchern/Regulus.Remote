namespace Regulus.Network.RUDP
{
    public class BufferDispenser
    {
        
        
        private readonly int _PayloadSize;

        private ushort _Serial;
        private int _PackageSize;

        public BufferDispenser(int package_size)
        {
            _PayloadSize = package_size - SegmentPackage.GetHeadSize();
            _PackageSize = package_size;
            
        }

        

        public SegmentPackage[] Packing(byte[] buffer,PEER_OPERATION operation)
        {            
            var count = buffer.Length / _PayloadSize + 1  ;
            var packages = new SegmentPackage[count];
            

            var buffserSize = buffer.Length;
            for (int i = count - 1; i >= 0; i--)
            {         
                var package = new SegmentPackage(_PackageSize);
                package.SetSeq((ushort)(_Serial + i));
                package.SetOperation((byte)operation);
                var begin = _PayloadSize * i;
                var writeSize = buffserSize - begin;
                package.WritePayload(buffer,begin, writeSize);
                buffserSize -= writeSize;
                packages[i] = package;
            }
            _Serial += (ushort)count;
            return packages;
        }
    }
}