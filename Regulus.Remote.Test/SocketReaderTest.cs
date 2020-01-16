using Regulus.Network;
using System;
using System.Net;

namespace Regulus.Remote.Tests
{
    public class SocketReaderTestPeer : IPeer
    {
        EndPoint IPeer.RemoteEndPoint => throw new NotImplementedException();

        EndPoint IPeer.LocalEndPoint => throw new NotImplementedException();

        bool IPeer.Connected => throw new NotImplementedException();

        void IPeer.Close()
        {
            throw new NotImplementedException();
        }

        void IPeer.Receive(byte[] buffer, int offset, int count, Action<int> done)
        {
            for(byte i= 0; i < 10; ++i)
            {
                buffer[offset + i] = i;
            }
            done(count);
        }

        Task IPeer.Send(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    public class SocketReaderTest
    {
        [NUnit.Framework.Test()]
        public void ScoketRead10ByteTest()
        {
            
            var peer = new SocketReaderTestPeer();
            

            var reader = new Regulus.Remote.SocketBodyReader(peer);
            byte[] readBytes = new byte[10];
            reader.DoneEvent += (data) =>
            {
                readBytes = data;
            };
            reader.Read(10);

            NUnit.Framework.Assert.AreEqual(1 , readBytes[1]);
            NUnit.Framework.Assert.AreEqual(5, readBytes[5]);
            NUnit.Framework.Assert.AreEqual(9, readBytes[9]);
        }
        [NUnit.Framework.Test()]
        public void ReadHeadTest()
        {
            var peer = new SocketHeadReaderTestPeer();

            var reader = new Regulus.Remote.SocketHeadReader(peer) ;
            var readEvent = reader as ISocketReader;
            var buffer = new System.Collections.Generic.List<byte>();
            readEvent.DoneEvent += (read_buffer) => {
                buffer.AddRange(read_buffer);
            };
            reader.Read();
            NUnit.Framework.Assert.AreEqual(0x85, buffer[0]);
            NUnit.Framework.Assert.AreEqual(0x05, buffer[1]);
            
        }
    }
    
 
}