using Regulus.Network.Package;
using System;
using System.Net;

namespace Regulus.Network
{
    public class BufferDispenser
    {


        private readonly int _PayloadSize;

        private ushort _Serial;
        private readonly EndPoint _EndPoint;
        private readonly SocketMessageFactory _Spawner;


        public BufferDispenser(EndPoint end_point, SocketMessageFactory spawner)
        {
            _EndPoint = end_point;
            _Spawner = spawner;
            _PayloadSize = SocketMessage.GetPayloadSize();
        }

        public int Serial { get { return _Serial; } }


        public SocketMessage[] PackingTransmission(byte[] bufgfer, ushort ack, uint ack_fields)
        {

            int count = (bufgfer.Length + _PayloadSize - 1) / _PayloadSize;
            SocketMessage[] packages = new SocketMessage[count];


            int buffserSize = bufgfer.Length;
            for (int i = count - 1; i >= 0; i--)
            {
                SocketMessage package = _Spawner.Spawn();
                package.SetEndPoint(_EndPoint);
                package.SetSeq((ushort)(_Serial + i));
                package.SetOperation((byte)PeerOperation.Transmission);
                package.SetAck(ack);
                package.SetAckFields(ack_fields);
                int begin = _PayloadSize * i;
                int writeSize = buffserSize - begin;
                package.WritePayload(bufgfer, begin, writeSize);
                buffserSize -= writeSize;
                packages[i] = package;
            }
            _Serial += (ushort)count;
            return packages;
        }

        public SocketMessage PackingAck(ushort ack, uint ack_fields)
        {
            SocketMessage package = _Spawner.Spawn();
            package.SetEndPoint(_EndPoint);
            package.SetOperation((byte)PeerOperation.Acknowledge);
            package.SetAck(ack);
            package.SetAckFields(ack_fields);
            package.ClearPayload();
            return package;
        }
        public SocketMessage PackingOperation(PeerOperation operation, ushort ack, uint ack_fields)
        {
            if (operation == PeerOperation.Acknowledge)
                throw new Exception("Ack type use PackingAck.");

            SocketMessage package = _Spawner.Spawn();
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