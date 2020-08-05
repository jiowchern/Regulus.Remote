using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Regulus.Network;
using Regulus.Utility;
namespace Regulus.Remote.Standalone
{

    public class CommunicationDevice : Network.IPeer
    {
		readonly System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Sends;
		readonly System.Collections.Concurrent.ConcurrentQueue<MemoryStream> _Receives;
		public CommunicationDevice()
        {
			
			_Sends = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();
			_Receives = new System.Collections.Concurrent.ConcurrentQueue<MemoryStream>();

		}

        public void Push(byte[] buffer)
        {
			_Receives.Enqueue(new MemoryStream(buffer,0, buffer.Length));
		}

        EndPoint IPeer.RemoteEndPoint => throw new NotImplementedException();

        EndPoint IPeer.LocalEndPoint => throw new NotImplementedException();

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

        public Task<MemoryStream> Pop()
        {
			return Task<MemoryStream>.Run(()=>{
                MemoryStream stream;
                while (!_Sends.TryDequeue(out stream))
                {

                }
				return stream;
			});

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
namespace Regulus.Remote.Ghost
{
	public partial class Agent : IAgent
	{
        private readonly IUpdatable _GhostSerializer;
        private readonly GhostProvider _GhostProvider;

		private long _Ping
		{
			get { return _GhostProvider.Ping; }
		}

	    public Agent(IProtocol protocol,Regulus.Network.IPeer peer)
	    {	    
            var serializer = protocol.GetSerialize();

			var ghostSerializer = new GhostSerializer(peer , serializer);

			_GhostProvider = new GhostProvider(protocol, ghostSerializer);

			_GhostSerializer = ghostSerializer;
		}

		bool IUpdatable.Update()
		{
			return _GhostSerializer.Update();
		}

		void IBootable.Launch()
		{
            Singleton<Log>.Instance.WriteInfo("Agent Launch.");
            _GhostProvider.ErrorMethodEvent += _ErrorMethodEvent;
		    _GhostProvider.ErrorVerifyEvent += _ErrorVerifyEvent;

			_GhostSerializer.Launch();
		}

		void IBootable.Shutdown()
		{
			_GhostProvider.Dispose();
			_GhostSerializer.Shutdown();
			_GhostProvider.ErrorVerifyEvent -= _ErrorVerifyEvent;
            _GhostProvider.ErrorMethodEvent -= _ErrorMethodEvent;

            Singleton<Log>.Instance.WriteInfo("Agent Shutdown.");
		}

		INotifier<T> INotifierQueryable.QueryNotifier<T>()
		{
			return _GhostProvider.QueryProvider<T>();
		}

		long IAgent.Ping
		{
			get { return _Ping; }
		}

	    private event Action<string, string> _ErrorMethodEvent;

	    event Action<string, string> IAgent.ErrorMethodEvent
	    {
	        add { this._ErrorMethodEvent += value; }
	        remove { this._ErrorMethodEvent -= value; }
	    }

	    private event Action<byte[], byte[]> _ErrorVerifyEvent;

	    event Action<byte[], byte[]> IAgent.ErrorVerifyEvent
	    {
	        add { this._ErrorVerifyEvent += value; }
	        remove { this._ErrorVerifyEvent -= value; }
	    }

	    bool IAgent.Connected
		{
			get { return _GhostProvider.Enable; }
		}
    }
}
