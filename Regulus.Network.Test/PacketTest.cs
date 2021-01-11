using Xunit;

namespace Regulus.Network.Tests
{
    public class PacketTest
    {
        [Xunit.Fact]
        public void Seq()
        {
            SocketMessageFactory spawner = SocketMessageFactory.Instance;
            Package.SocketMessage buffer = spawner.Spawn();
            buffer.SetSeq(0x1234);
            ushort seq = buffer.GetSeq();
            Assert.Equal((ushort)0x1234, seq);
        }


        public void Ack()
        {
            SocketMessageFactory spawner = SocketMessageFactory.Instance;
            Package.SocketMessage buffer = spawner.Spawn();
            buffer.SetAck(0x1234);
            ushort value = buffer.GetAck();
            Assert.Equal((ushort)0x1234, value);
        }


        public void AckFields()
        {
            SocketMessageFactory spawner = SocketMessageFactory.Instance;
            Package.SocketMessage buffer = spawner.Spawn();
            buffer.SetAckFields(0x12345678u);
            uint value = buffer.GetAckFields();
            Assert.Equal((uint)0x12345678, value);
        }


        public void Operation()
        {
            SocketMessageFactory spawner = SocketMessageFactory.Instance;
            Package.SocketMessage buffer = spawner.Spawn();
            buffer.SetOperation(0x12);
            byte value = buffer.GetOperation();
            Assert.Equal((byte)0x12, value);
        }




        public void Payload()
        {
            SocketMessageFactory spawner = SocketMessageFactory.Instance;
            Package.SocketMessage buffer = spawner.Spawn();
            int payloadSize = buffer.GetPayloadBufferSize();
            byte[] payloadSource = new byte[payloadSize];
            byte[] payloadReaded = new byte[payloadSize];

            _BuildPayloadData(payloadSource);
            buffer.WritePayload(payloadSource, 0, payloadSource.Length);

            bool ok = buffer.CheckPayload();
            Assert.True(ok);
            ushort payloadLength = buffer.GetPayloadLength();
            Assert.Equal((ushort)payloadSize, payloadLength);


            bool result = buffer.ReadPayload(payloadReaded, 0);
            Assert.True(result);
            for (int i = 0; i < payloadLength; ++i)
            {
                Assert.Equal((byte)i, payloadReaded[i]);
            }
        }

        private void _BuildPayloadData(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; ++i)
            {
                buffer[i] = (byte)i;
            }
        }
    }
}
