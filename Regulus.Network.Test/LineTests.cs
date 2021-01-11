using Xunit;
using Regulus.Network.Package;
using System.Collections.Generic;
using System.Net;

namespace Regulus.Network.RUDP.Tests
{

    public class LineTests
    {
        [Xunit.Fact]
        public void LineTest()
        {
            Line line = new Line(new IPEndPoint(0, 0));
            Assert.Equal(0, line.AcknowledgeCount);
        }

        [Xunit.Fact]
        public void WriteTest()
        {
            SocketMessage message = null;
            Line line = new Line(new IPEndPoint(0, 0));
            line.OutputEvent += msg => { message = msg; };
            line.WriteTransmission(_CreateBuffer(1));
            line.Tick(new Timestamp(0, 0));
            Assert.Equal(1, line.AcknowledgeCount);
            Assert.Equal(0, message.GetSeq());

        }

        private byte[] _CreateBuffer(int count)
        {
            List<byte> buffer = new List<byte>();
            int length = SocketMessage.GetPayloadSize() * count;
            for (int i = 0; i < length; i++)
            {
                buffer.Add((byte)i);
            }
            return buffer.ToArray();
        }


        [Xunit.Fact]
        public void WriteResendTest()
        {
            List<SocketMessage> messages = new List<SocketMessage>();
            Line line = new Line(new IPEndPoint(0, 0));
            line.OutputEvent += msg => { messages.Add(msg); };
            line.WriteTransmission(_CreateBuffer(1));
            line.Tick(new Timestamp(0, 0));
            Assert.Equal(1, line.AcknowledgeCount);
            Assert.Equal(0, messages[0].GetSeq());
            line.Tick(new Timestamp(Timestamp.OneSecondTicks * 3, Timestamp.OneSecondTicks * 3));

            Assert.Equal(1, line.AcknowledgeCount);
            Assert.Equal(2, messages.Count);
        }

        [Xunit.Fact]
        public void ReadWriteTest()
        {
            List<SocketMessage> messages1 = new List<SocketMessage>();
            Line line1 = new Line(new IPEndPoint(0, 0));
            line1.OutputEvent += msg => { messages1.Add(msg); };

            List<SocketMessage> messages2 = new List<SocketMessage>();
            Line line2 = new Line(new IPEndPoint(1, 0));
            line2.OutputEvent += msg => { messages2.Add(msg); };

            byte[] payload = _CreateBuffer(1);
            line1.WriteTransmission(payload);

            line1.Tick(new Timestamp(1, 1));
            SocketMessage messageTransmission = messages1[0];

            line2.Input(messageTransmission);
            line2.Tick(new Timestamp(2, 1));
            SocketMessage messageAck = messages2[0];

            line1.Input(messageAck);
            line1.Tick(new Timestamp(3, 1));



            Assert.Equal(0, line1.AcknowledgeCount);
            Assert.Equal(0, line2.AcknowledgeCount);
        }

        [Xunit.Fact]
        public void InputTest()
        {
            List<SocketMessage> messages = new List<SocketMessage>();
            Line line = new Line(new IPEndPoint(0, 0));
            line.OutputEvent += msg => { messages.Add(msg); };
            line.WriteTransmission(_CreateBuffer(1));
            line.Tick(new Timestamp(0, 0));

            SocketMessage message = new SocketMessage(Config.Default.PackageSize);
            message.SetSeq(0);
            message.SetOperation((byte)PeerOperation.Acknowledge);
            message.SetAck(1);
            message.SetAckFields(0);
            line.Input(message);
            line.Tick(new Timestamp(1, 1));
            Assert.Equal(1, line.AcknowledgeCount);
        }


    }
}