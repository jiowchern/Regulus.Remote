using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Regulus.Network.RUDP;

namespace Regulus.Network.Tests
{
    
    public class PacketTest
    {
        [Test]
        public void Seq()
        {
            ISocketPackageSpawner spawner = SocketPackagePool.Instance;
            var buffer = spawner.Spawn();
            buffer.SetSeq(0x1234);
            var seq = buffer.GetSeq();
            Assert.AreEqual((ushort)0x1234, seq);
        }

        
        public void Ack()
        {
            ISocketPackageSpawner spawner = SocketPackagePool.Instance;
            var buffer = spawner.Spawn();
            buffer.SetAck(0x1234);
            var value = buffer.GetAck();
            Assert.AreEqual((ushort)0x1234, value);
        }

        
        public void AckFields()
        {
            ISocketPackageSpawner spawner = SocketPackagePool.Instance;
            var buffer = spawner.Spawn();
            buffer.SetAckFields(0x12345678u);
            var value = buffer.GetAckFields();
            Assert.AreEqual((uint)0x12345678, value);
        }

        
        public void Operation()
        {
            ISocketPackageSpawner spawner = SocketPackagePool.Instance;
            var buffer = spawner.Spawn();
            buffer.SetOperation(0x12);
            var value = buffer.GetOperation();
            Assert.AreEqual((byte)0x12, value);
        }
        


        
        public void Payload()
        {
            ISocketPackageSpawner spawner = SocketPackagePool.Instance;
            var buffer = spawner.Spawn();
            var payloadSize = buffer.GetPayloadBufferSize();
            var payloadSource = new byte[payloadSize];
            var payloadReaded = new byte[payloadSize];

            _BuildPayloadData(payloadSource);
            buffer.WritePayload(payloadSource ,0 , payloadSource.Length);

            var ok = buffer.CheckPayload();
            Assert.IsTrue(ok);
            var payloadLength = buffer.GetPayloadLength();
            Assert.AreEqual((ushort)payloadSize , payloadLength);

            
            var result = buffer.ReadPayload(payloadReaded , 0);
            Assert.IsTrue(result);
            for (int i = 0; i < payloadLength; ++i)
            {                
                Assert.AreEqual((byte) i, payloadReaded[i]);
            }            
        }

        private void _BuildPayloadData(byte[] buffer)
        {
            for(int i = 0 ; i < buffer.Length ; ++i)
            {
                buffer[i] = (byte)i;
            }
        }
    }
}
