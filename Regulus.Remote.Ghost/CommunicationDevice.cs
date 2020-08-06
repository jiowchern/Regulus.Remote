using System;
using System.IO;
using System.Threading.Tasks;
using Regulus.Network;
using Regulus.Utility;
namespace Regulus.Remote.Standalone
{

    public class ReversePeer : Network.IPeer
    {
        private readonly CommunicationDevice peer;

        public ReversePeer(CommunicationDevice peer)
        {
            this.peer = peer;
        }
		bool IPeer.Connected => throw new NotImplementedException();

        void IPeer.Close()
        {
            throw new NotImplementedException();
        }

        Task<int> IPeer.Receive(byte[] buffer, int offset, int count)
        {
			return peer.Pop(buffer , offset, count);

		}

        Task<int> IPeer.Send(byte[] buffer, int offset, int count)
        {
			return peer.Push(buffer, offset, count);
		}
    }


    public class CommunicationDevice : Network.IPeer
    {
		readonly System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Sends;
		readonly System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Receives;
		public CommunicationDevice()
        {
			
			_Sends = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();
			_Receives = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();

		}

        public Task<int> Push(byte[] buffer,int offset , int count)
        {
			_Receives.Enqueue(new MemoryStream(buffer, offset, count));
			return Task<int>.FromResult(count);
		}


        bool IPeer.Connected => throw new NotImplementedException();

        void IPeer.Close()
        {
            throw new NotImplementedException();
        }

        Task<int> IPeer.Receive(byte[] buffer, int offset, int count)
        {
			MemoryStream stream;			
			if (_Receives.TryPeek(out stream))
			{
				var streamCount = stream.Length - stream.Position;
				var readCount = count;
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
				var streamCount = stream.Length - stream.Position;
				var readCount = count;
				if (streamCount <= readCount)
					_Sends.TryDequeue(out stream);
				return stream.ReadAsync(buffer, offset, count);
			}
			return System.Threading.Tasks.Task<int>.FromResult(0);

		}

        Task<int> IPeer.Send(byte[] buffer, int offset, int count)
        {
			return Task<int>.Run(() =>
			{
				var stream = new MemoryStream(buffer, offset, count);
				_Sends.Enqueue(stream);
				return (int)stream.Length;
			});			
		}
    }
}
