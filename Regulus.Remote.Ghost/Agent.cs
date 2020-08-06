using System;
using System.Collections.Generic;
using System.Net;
using Regulus.Network;
using Regulus.Utility;
namespace Regulus.Remote.Standalone
{
}
namespace Regulus.Remote.Ghost
{
    public partial class Agent : IAgent
	{
        
        private readonly GhostProvider _GhostProvider;
        private readonly GhostSerializer _GhostSerializer;

        private long _Ping
		{
			get { return _GhostProvider.Ping; }
		}

	    public Agent(IProtocol protocol)
	    {	    
            var serializer = protocol.GetSerialize();

			var ghostSerializer = new GhostSerializer(serializer);

			_GhostProvider = new GhostProvider(protocol, ghostSerializer);

			_GhostSerializer = ghostSerializer;
		}

		public void Update()
		{
			_GhostSerializer.Update();
		}

		void IAgent.Start(IPeer peer)
		{
            Singleton<Log>.Instance.WriteInfo("Agent Launch.");
            _GhostProvider.ErrorMethodEvent += _ErrorMethodEvent;
		    _GhostProvider.ErrorVerifyEvent += _ErrorVerifyEvent;
			_GhostSerializer.Start(peer);
			_GhostProvider.Start();			
		}

		void IAgent.Stop()
		{
			_GhostProvider.Stop();
			_GhostSerializer.Stop();
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

	    
    }
}
