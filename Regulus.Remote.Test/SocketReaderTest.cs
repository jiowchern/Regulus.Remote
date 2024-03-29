﻿using Regulus.Network;
using System;
using System.Threading.Tasks;

namespace Regulus.Remote.Tests
{
    public class SocketReaderTestPeer : IStreamable
    {



        IWaitableValue<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            return System.Threading.Tasks.Task<int>.Run(() =>
            {
                for (byte i = 0; i < 10; ++i)
                {
                    buffer[offset + i] = i;
                }
                return 10;
            }).ToWaitableValue();

        }

        IWaitableValue<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    public class SocketReaderTest
    {
        [NUnit.Framework.Test(), NUnit.Framework.Timeout(5000)]

        public void ScoketRead10ByteTest()
        {

            SocketReaderTestPeer peer = new SocketReaderTestPeer();


            SocketBodyReader reader = new Regulus.Remote.SocketBodyReader(peer);
            
            var readBytes = new System.Collections.Generic.List<byte>();
            reader.DoneEvent += (data) =>
            {
                readBytes.AddRange(data);
            };
            reader.Read(10);
            Regulus.Utility.IStatus status = reader;
            status.Enter();
            
            while (readBytes.Count < 10)
            {
                status.Update();
            }
            status.Leave();
            NUnit.Framework.Assert.AreEqual(1, readBytes[1]);
            NUnit.Framework.Assert.AreEqual(5, readBytes[5]);
            NUnit.Framework.Assert.AreEqual(9, readBytes[9]);
        }
        [NUnit.Framework.Test(), NUnit.Framework.Timeout(5000)]

        public void ReadHeadTest()
        {
            SocketHeadReaderTestPeer peer = new SocketHeadReaderTestPeer();

            SocketHeadReader reader = new Regulus.Remote.SocketHeadReader(peer);
            ISocketReader readEvent = reader as ISocketReader;
            var buffer = new System.Collections.Generic.List<byte>();
            readEvent.DoneEvent += (read_buffer) =>
            {
                buffer.AddRange(read_buffer);
            };

            reader.Read();
            readEvent.Enter();            
            while (buffer.Count != 2)
            {
                readEvent.Update();
            }
            readEvent.Leave();
            NUnit.Framework.Assert.AreEqual(0x85, buffer[0]);
            NUnit.Framework.Assert.AreEqual(0x05, buffer[1]);

        }
    }


}