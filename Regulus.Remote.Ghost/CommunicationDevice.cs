using Regulus.Network;
using System.IO;
using System.Threading.Tasks;
namespace Regulus.Remote.Standalone
{


    public class Stream : Network.IStreamable
    {
        readonly System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Sends;
        readonly System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Receives;
        public Stream()
        {

            _Sends = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();
            _Receives = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();

        }

        public Task<int> Push(byte[] buffer, int offset, int count)
        {
            _Receives.Enqueue(new MemoryStream(buffer, offset, count));
            return Task<int>.FromResult(count);
        }




        Task<int> IStreamable.Receive(byte[] buffer, int offset, int count)
        {
            MemoryStream stream;
            if (_Receives.TryPeek(out stream))
            {
                long streamCount = stream.Length - stream.Position;
                int readCount = count;
                if (streamCount <= readCount)
                    _Receives.TryDequeue(out stream);
                return stream.ReadAsync(buffer, offset, count);
            }
            return System.Threading.Tasks.Task<int>.FromResult(0);
        }



        public Task<int> Pop(byte[] buffer, int offset, int count)
        {
            MemoryStream stream;
            if (_Sends.TryPeek(out stream))
            {
                long streamCount = stream.Length - stream.Position;
                int readCount = count;
                if (streamCount <= readCount)
                    _Sends.TryDequeue(out stream);
                return stream.ReadAsync(buffer, offset, count);
            }
            return System.Threading.Tasks.Task<int>.FromResult(0);

        }

        Task<int> IStreamable.Send(byte[] buffer, int offset, int count)
        {
            return Task<int>.Run(() =>
            {
                MemoryStream stream = new MemoryStream(buffer, offset, count);
                _Sends.Enqueue(stream);
                return (int)stream.Length;
            });
        }
    }
}
