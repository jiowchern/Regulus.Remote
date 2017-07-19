using System.Net;

namespace Regulus.Network.RUDP
{
    public class BufferDispenser
    {
        
        
        private readonly int _PayloadSize;

        private ushort _Serial;
        private readonly EndPoint _EndPoint;
        private readonly ISocketPackageSpawner _Spawner;


        public BufferDispenser(EndPoint end_point, ISocketPackageSpawner spawner)
        {
            _EndPoint = end_point;
            _Spawner = spawner;
            _PayloadSize = SocketMessage.GetPayloadSize();
        }

        

        public SocketMessage[] Packing(byte[] buffer,PEER_OPERATION operation)
        {            
            var count = buffer.Length / _PayloadSize + 1  ;
            var packages = new SocketMessage[count];
            

            var buffserSize = buffer.Length;
            for (int i = count - 1; i >= 0; i--)
            {

                var package = _Spawner.Spawn();
                package.SetEndPoint(_EndPoint);
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