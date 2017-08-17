
using Regulus.Network.RUDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Regulus.Network.RUDP.Tests
{
    
    public class LineTests
    {
        [Test]
        public void LineTest()
        {
            var line = new Line(new IPEndPoint(0,0));            
            Assert.AreEqual(0, line.AcknowledgeCount);
        }

        [Test]
        public void WriteTest()
        {
            SocketMessage message = null;
            var line = new Line(new IPEndPoint(0, 0));
            line.OutputEvent += msg => { message = msg; };
            line.WriteTransmission(_CreateBuffer(1));            
            line.Tick(new Timestamp(0, 0));
            Assert.AreEqual(1, line.AcknowledgeCount);
            Assert.AreEqual(0, message.GetSeq());
            
        }

        private byte[] _CreateBuffer(int count)
        {
            List<byte> buffer = new List<byte>();
            var length = SocketMessage.GetPayloadSize() * count;
            for (int i = 0; i < length; i++)
            {
                buffer.Add((byte)i);
            }
            return buffer.ToArray();
        }


        [Test]
        public void WriteResendTest()
        {
            List<SocketMessage> messages = new List<SocketMessage>();
            var line = new Line(new IPEndPoint(0, 0));
            line.OutputEvent += msg => { messages.Add(msg); };
            line.WriteTransmission(_CreateBuffer(1));
            line.Tick(new Timestamp(0, 0));
            Assert.AreEqual(1, line.AcknowledgeCount);            
            Assert.AreEqual(0, messages[0].GetSeq());
            line.Tick(new Timestamp(Timestamp.OneSecondTicks * 3 , Timestamp.OneSecondTicks * 3));
            
            Assert.AreEqual(1, line.AcknowledgeCount);
            Assert.AreEqual(2, messages.Count);
        }

        [Test]
        public void ReadWriteTest()
        {
            List<SocketMessage> messages1 = new List<SocketMessage>();
            var line1 = new Line(new IPEndPoint(0, 0));
            line1.OutputEvent += msg => { messages1.Add(msg); };

            List<SocketMessage> messages2 = new List<SocketMessage>();
            var line2 = new Line(new IPEndPoint(1, 0));
            line2.OutputEvent += msg => { messages2.Add(msg); };

            var payload = _CreateBuffer(1);
            line1.WriteTransmission(payload);
            
            line1.Tick(new Timestamp(1, 1));
            var messageTransmission = messages1[0];

            line2.Input(messageTransmission);
            line2.Tick(new Timestamp(2, 1));
            var messageAck = messages2[0];

            line1.Input(messageAck);
            line1.Tick(new Timestamp(3, 1));

            

            Assert.AreEqual(0, line1.AcknowledgeCount);
            Assert.AreEqual(0, line2.AcknowledgeCount);
        }

        [Test]
        public void InputTest()
        {
            List<SocketMessage> messages = new List<SocketMessage>();
            var line = new Line(new IPEndPoint(0, 0));
            line.OutputEvent += msg => { messages.Add(msg); };
            line.WriteTransmission(_CreateBuffer(1));
            line.Tick(new Timestamp(0, 0));

            var message = new SocketMessage(Config.Default.PackageSize);
            message.SetSeq(0);
            message.SetOperation((byte)PEER_OPERATION.ACKNOWLEDGE);
            message.SetAck(1);
            message.SetAckFields(0);
            line.Input(message);
            line.Tick(new Timestamp(1, 1));
            Assert.AreEqual(1, line.AcknowledgeCount);
        }

        
    }
}