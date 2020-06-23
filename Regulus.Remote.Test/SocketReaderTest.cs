using Regulus.Network;
using System;
using System.Net;
using System.Threading.Tasks;

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

        Task<int> IPeer.Receive(byte[] buffer, int offset, int count)
        {
            return System.Threading.Tasks.Task<int>.Run(() => {
                for (byte i = 0; i < 10; ++i)
                {
                    buffer[offset + i] = i;
                }
                return 10;
            });
            
        }

        SendTask IPeer.Send(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    public class SocketReaderTest
    {
        [NUnit.Framework.Test()]
        [NUnit.Framework.MaxTime(5000)]
        public void ScoketRead10ByteTest()
        {
            
            var peer = new SocketReaderTestPeer();
            

            var reader = new Regulus.Remote.SocketBodyReader(peer);
            byte[] readBytes = new byte[10];
            bool readed = false;
            reader.DoneEvent += (data) =>
            {
                readed = true;
                readBytes = data;
            };
            reader.Read(10);

            while(!readed )
            {

            }
            NUnit.Framework.Assert.AreEqual(1 , readBytes[1]);
            NUnit.Framework.Assert.AreEqual(5, readBytes[5]);
            NUnit.Framework.Assert.AreEqual(9, readBytes[9]);
        }
        [NUnit.Framework.Test()]
        [NUnit.Framework.MaxTime(5000)]
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
            while(buffer.Count != 2)
            {

            }
            NUnit.Framework.Assert.AreEqual(0x85, buffer[0]);
            NUnit.Framework.Assert.AreEqual(0x05, buffer[1]);
            
        }
    }
    
 
}