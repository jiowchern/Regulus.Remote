using Regulus.Network;
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
        [Xunit.Fact(Timeout =5000)]
        
        public async void ScoketRead10ByteTest()
        {

            SocketReaderTestPeer peer = new SocketReaderTestPeer();


            SocketBodyReader reader = new Regulus.Remote.SocketBodyReader(peer);
            
            var readBytes = new System.Collections.Generic.List<byte>();
            reader.DoneEvent += (data) =>
            {
                readBytes.AddRange(data);
            };
            reader.Read(10);
            
            while (readBytes.Count < 10) ;
            Xunit.Assert.Equal(1, readBytes[1]);
            Xunit.Assert.Equal(5, readBytes[5]);
            Xunit.Assert.Equal(9, readBytes[9]);
        }
        [Xunit.Fact(Timeout =5000)]
        
        public void ReadHeadTest()
        {
            SocketHeadReaderTestPeer peer = new SocketHeadReaderTestPeer();

            SocketHeadReader reader = new Regulus.Remote.SocketHeadReader(peer);
            ISocketReader readEvent = reader as ISocketReader;
            System.Collections.Generic.List<byte> buffer = new System.Collections.Generic.List<byte>();
            readEvent.DoneEvent += (read_buffer) =>
            {
                buffer.AddRange(read_buffer);
            };
            reader.Read();
            while (buffer.Count != 2)
            {

            }
            Xunit.Assert.Equal(0x85, buffer[0]);
            Xunit.Assert.Equal(0x05, buffer[1]);

        }
    }


}