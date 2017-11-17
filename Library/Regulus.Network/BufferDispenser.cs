using System;
using System.Net;
using Regulus.Network.Package;

namespace Regulus.Network
{
    public class BufferDispenser
    {
        
        
        private readonly int m_PayloadSize;

        private ushort m_Serial;
        private readonly EndPoint m_EndPoint;
        private readonly SocketMessageFactory m_Spawner;


        public BufferDispenser(EndPoint EndPoint, SocketMessageFactory Spawner)
        {
            m_EndPoint = EndPoint;
            m_Spawner = Spawner;
            m_PayloadSize = SocketMessage.GetPayloadSize();
        }

        public int Serial => m_Serial;


        public SocketMessage[] PackingTransmission(byte[] Buffer,ushort Ack ,uint AckFields )
        {
            
            var count = (Buffer.Length + m_PayloadSize - 1) / m_PayloadSize   ;
            var packages = new SocketMessage[count];
            

            var buffserSize = Buffer.Length;
            for (var i = count - 1; i >= 0; i--)
            {
                var package = m_Spawner.Spawn();
                package.SetEndPoint(m_EndPoint);
                package.SetSeq((ushort)(m_Serial + i));
                package.SetOperation((byte)PeerOperation.Transmission);
                package.SetAck(Ack);
                package.SetAckFields(AckFields);
                var begin = m_PayloadSize * i;
                var writeSize = buffserSize - begin;
                package.WritePayload(Buffer,begin, writeSize);
                buffserSize -= writeSize;
                packages[i] = package;
            }
            m_Serial += (ushort)count;
            return packages;
        }

        public SocketMessage PackingAck(ushort Ack, uint AckFields)
        {
            var package = m_Spawner.Spawn();
            package.SetEndPoint(m_EndPoint);            
            package.SetOperation((byte)PeerOperation.Acknowledge);
            package.SetAck(Ack);
            package.SetAckFields(AckFields);
            package.ClearPayload();
            return package;
        }
        public SocketMessage PackingOperation(PeerOperation Operation, ushort Ack, uint AckFields)
        {
            if(Operation == PeerOperation.Acknowledge)
                throw new Exception("Ack type use PackingAck.");

            var package = m_Spawner.Spawn();
            package.SetEndPoint(m_EndPoint);
            package.SetSeq(m_Serial++);
            package.SetOperation((byte)Operation);
            package.SetAck(Ack);
            package.SetAckFields(AckFields);
            package.ClearPayload();


            return package;
        }
    }
}