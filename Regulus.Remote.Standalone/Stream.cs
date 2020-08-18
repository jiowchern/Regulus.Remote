using Regulus.Network;
using System.IO;
using System.Threading.Tasks;
namespace Regulus.Remote.Standalone
{


    public class Stream : Network.IStreamable
    {
        volatile System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Sends;
        volatile System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Receives;
        public Stream()
        {

            _Sends = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();
            _Receives = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();

        }

        public Task<int> Push(byte[] buffer, int offset, int count)
        {
            var stream = new MemoryStream(buffer, offset, count);
            _Receives.Enqueue(stream);
            return Task<int>.FromResult((int)stream.Length);
        }




        Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            
            return _Read(buffer, offset, count,_Receives );
        }

        private Task<int> _Read(byte[] buffer, int offset, int count , System.Collections.Concurrent.ConcurrentQueue<MemoryStream> read_queue)
        {
            MemoryStream stream1;
            MemoryStream stream2;
            if (read_queue.TryPeek(out stream1))
            {
                                
                var readSize = stream1.Read(buffer, offset, count);
                if (stream1.Length == stream1.Position)
                {
                    read_queue.TryDequeue(out stream2);
                    if (stream1 != stream2)
                        throw new System.Exception("stream1 != stream2");
                }

                return System.Threading.Tasks.Task<int>.FromResult(readSize);
            }
            return System.Threading.Tasks.Task<int>.FromResult(0);
        }

        public Task<int> Pop(byte[] buffer, int offset, int count)
        {
            return _Read(buffer, offset, count, _Sends);


        }

        Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            MemoryStream stream = new MemoryStream(buffer, offset, count);
            _Sends.Enqueue(stream);
            return Task<int>.FromResult((int)stream.Length);
        }
    }
}
