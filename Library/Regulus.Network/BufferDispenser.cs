using System;
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

        public int Serial { get { return _Serial; }}


        public SocketMessage[] PackingTransmission(byte[] buffer,ushort ack ,uint ack_fields )
        {
            
            var count = (buffer.Length + _PayloadSize - 1) / _PayloadSize   ;
            var packages = new SocketMessage[count];
            

            var buffserSize = buffer.Length;
            for (int i = count - 1; i >= 0; i--)
            {
                var package = _Spawner.Spawn();
                package.SetEndPoint(_EndPoint);
                package.SetSeq((ushort)(_Serial + i));
                package.SetOperation((byte)PEER_OPERATION.TRANSMISSION);
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

        public SocketMessage PackingAck(ushort ack, uint ack_fields)
        {
            var package = _Spawner.Spawn();
            package.SetEndPoint(_EndPoint);            
            package.SetOperation((byte)PEER_OPERATION.ACKNOWLEDGE);
            package.SetAck(ack);
            package.SetAckFields(ack_fields);
            package.ClearPayload();
            return package;
        }
        public SocketMessage PackingOperation(PEER_OPERATION operation, ushort ack, uint ack_fields)
        {
            if(operation == PEER_OPERATION.ACKNOWLEDGE)
                throw new Exception("Ack type use PackingAck.");

            var package = _Spawner.Spawn();
            package.SetEndPoint(_EndPoint);
            package.SetSeq(_Serial++);
            package.SetOperation((byte)operation);
            package.SetAck(ack);
            package.SetAckFields(ack_fields);
            package.ClearPayload();


            return package;
        }
    }
}