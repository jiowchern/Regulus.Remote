using Regulus.Network;
using System.IO;
using System.Threading.Tasks;
namespace Regulus.Remote.Standalone
{


    public class Stream : Network.IStreamable
    {
        readonly System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Sends;
        readonly System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Receives;
        int _DebugSendCount;
        int _DebugReceiveCount;
        public Stream()
        {

            _Sends = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();
            _Receives = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();

        }

        public Task<int> Push(byte[] buffer, int offset, int count)
        {
            var stream = new MemoryStream(buffer, offset, count);
            
            _Receives.Enqueue(stream);

            System.Threading.Interlocked.Increment(ref _DebugReceiveCount);
            return System.Threading.Tasks.Task<int>.Factory.StartNew(()=> (int)stream.Length);
        }




        Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            
            return _Read(buffer, offset, count);
        }

        private Task<int> _Read(byte[] buffer, int offset, int count )
        {
            System.Func<int> reader = () => {
                MemoryStream stream1;
                MemoryStream stream2;
                var are = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
                int readSize = 0;
                while(readSize == 0)
                {
                    
                    if (_Receives.TryPeek(out stream1))
                    {
                        readSize += stream1.Read(buffer, offset, count);
                        if (stream1.Length == stream1.Position)
                        {
                            
                            _Receives.TryDequeue(out stream2);
                            System.Threading.Interlocked.Decrement(ref _DebugReceiveCount);
                            if (stream1 != stream2)
                                throw new System.Exception("stream1 != stream2");
                        }
                        return readSize;
                    }
                    are.Operate();
                }
                
                return readSize;
            };
            
            
            return System.Threading.Tasks.Task<int>.Factory.StartNew(reader);
        }

        private Task<int> _Write(byte[] buffer, int offset, int count)
        {
            System.Func<int> reader = () => {
                MemoryStream stream1;
                MemoryStream stream2;
                var are = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
                int readSize = 0;
                while (readSize == 0)
                {

                    if (_Sends.TryPeek(out stream1))
                    {
                        readSize += stream1.Read(buffer, offset, count);
                        if (stream1.Length == stream1.Position)
                        {
                            _Sends.TryDequeue(out stream2);
                            System.Threading.Interlocked.Decrement(ref _DebugSendCount);
                            if (stream1 != stream2)
                                throw new System.Exception("stream1 != stream2");
                        }
                        return readSize;
                    }
                    are.Operate();
                }

                return readSize;
            };


            return System.Threading.Tasks.Task<int>.Factory.StartNew(reader);
        }

        public Task<int> Pop(byte[] buffer, int offset, int count)
        {
            return _Write(buffer, offset, count);


        }

        Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            MemoryStream stream = new MemoryStream(buffer, offset, count);
            _Sends.Enqueue(stream);
            System.Threading.Interlocked.Increment(ref _DebugSendCount);
            return System.Threading.Tasks.Task<int>.Factory.StartNew(() => (int)stream.Length);
        }
    }
}
