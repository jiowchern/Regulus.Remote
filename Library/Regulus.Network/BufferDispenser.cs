using System.CodeDom;
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

        

        public SocketMessage[] Packing(byte[] buffer,PEER_OPERATION operation,ushort ack ,uint ack_fields )
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
                package.SetAck(ack);
                package.SetAckFields(ack_fields);
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