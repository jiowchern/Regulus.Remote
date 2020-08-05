
using System;

using System.Collections.Generic;

using Regulus.Utiliey;
using Regulus.Utility;
using Regulus.Serialization;

namespace Regulus.Remote.Standalone
{
	public class Agent : IRequestQueue, IResponseQueue, IBinder, IAgent
	{
		public delegate void ConnectedCallback();

		private event Action _BreakEvent;

		private event Action _ConnectEvent;

		private readonly GhostProvider _Agent;

		private readonly GhostRequest _GhostRequest;

		private readonly SoulProvider _SoulProvider;

		private readonly TProvider<IConnect> _ConnectProvider;
		private readonly TProvider<IOnline> _OnlineProvider;
		private readonly Regulus.Utility.StatusMachine _Machine;


		readonly System.Collections.Concurrent.ConcurrentQueue<System.Tuple<ServerToClientOpCode, byte[]>> _ResponseCodes;

		private bool _Connected;

		private IBinder _Binder
		{
			get { return _SoulProvider; }
		}

		public long Ping
		{
			get { return _Agent.Ping; }
		}

	    public Agent(IProtocol protocol)
	    {
			_Machine = new StatusMachine();
			_GhostRequest = new GhostRequest(protocol.GetSerialize());
            _Agent = new GhostProvider(protocol);
            _SoulProvider = new SoulProvider(this, this, protocol);
            _ResponseCodes = new System.Collections.Concurrent.ConcurrentQueue<Tuple<ServerToClientOpCode, byte[]>>();

			_ConnectProvider = new TProvider<IConnect>();
			_OnlineProvider = new TProvider<IOnline>();

			_Agent.AddProvider(typeof(IConnect), _ConnectProvider);
			_Agent.AddProvider(typeof(IOnline), _OnlineProvider);
			_ConnectEvent += () => { };
			_BreakEvent += () => { };
		}
        

		INotifier<T> INotifierQueryable.QueryNotifier<T>()
		{
			return QueryProvider<T>();
		}

		

		long IAgent.Ping
		{
			get { return _Agent.Ping; }
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

	    bool IUpdatable.Update()
		{
			_Machine.Update();
			_Update();			
			return true;
		}

		void IBootable.Launch()
		{
			Launch();
		}

		void IBootable.Shutdown()
		{
			_Machine.Termination();
			Shutdown();
		}

		event Action IAgent.ConnectEvent
		{
			add { _ConnectEvent += value; }
			remove { _ConnectEvent -= value; }
		}

		event Action IAgent.BreakEvent
		{
			add { _BreakEvent += value; }
			remove { _BreakEvent -= value; }
		}

		bool IAgent.Connected
		{
			get { return _Connected; }
		}

		event InvokeMethodCallback IRequestQueue.InvokeMethodEvent
		{
			add { _GhostRequest.CallMethodEvent += value; }
			remove { _GhostRequest.CallMethodEvent -= value; }
		}

		event Action IRequestQueue.BreakEvent
		{
			add { _BreakEvent += value; }
			remove { _BreakEvent -= value; }
		}

		

		void IResponseQueue.Push(ServerToClientOpCode cmd, byte[] data)
		{
            _ResponseCodes.Enqueue(new Tuple<ServerToClientOpCode, byte[]>(cmd,data));

	    }

		void IBinder.Return<TSoul>(TSoul soul)
		{
			_Binder.Return(soul);
		}

		void IBinder.Bind<TSoul>(TSoul soul)
		{
			_Bind(soul);
		}

		void IBinder.Unbind<TSoul>(TSoul soul)
		{
			_Unbind(soul);
		}

		event Action IBinder.BreakEvent
		{
			add { _BreakEvent += value; }
			remove { _BreakEvent -= value; }
		}

		public void Launch()
		{
			

			_GhostRequest.PingEvent += _OnRequestPing;
			_GhostRequest.ReleaseEvent += _SoulProvider.Unbind;
			_GhostRequest.SetPropertyDoneEvent += _SoulProvider.SetPropertyDone;
			_GhostRequest.AddEventEvent += _SoulProvider.AddEvent;
			_GhostRequest.RemoveEventEvent += _SoulProvider.RemoveEvent;

			_GhostRequest.AddNotifierSupplyEvent += _SoulProvider.AddNotifierSupply;
			_GhostRequest.RemoveNotifierSupplyEvent += _SoulProvider.RemoveNotifierSupply;
			_GhostRequest.AddNotifierUnsupplyEvent += _SoulProvider.AddNotifierUnsupply;
			_GhostRequest.RemoveNotifierUnsupplyEvent += _SoulProvider.RemoveNotifierUnsupply;


			_Agent.ErrorMethodEvent += _ErrorMethodEvent;
		    _Agent.ErrorVerifyEvent += _ErrorVerifyEvent;
            _Agent.Initial(_GhostRequest);

			_ToOffline();
		}
		private void _ToOffline()
		{
			var status = new OfflineStatus(_ConnectProvider);
			status.DoneEvent += ()=> {
				_ConnectEvent();
				_ToOnline();
			};
			_Machine.Push(status);
		}

		private void _ToOnline()
		{
			var status = new OnlineStatus(_OnlineProvider , new OnlineGhost(_Agent)) ;
			status.Ghost.DisconnectEvent +=()=> {
				_BreakEvent();
				_ToOffline();
			} ;			
			_Machine.Push(status);
		}

		private void _OnRequestPing()
		{
			_Agent.OnResponse(ServerToClientOpCode.Ping, new byte[0]);
		}

		private void _Update()
		{
            Tuple<ServerToClientOpCode, byte[]> code;
            if(_ResponseCodes.TryDequeue(out code ))
            {
                _Agent.OnResponse(code.Item1, code.Item2);

            }

            _SoulProvider.Update();
			_GhostRequest.Update();
		}

		public void Shutdown()
		{
		    _Agent.ErrorVerifyEvent -= _ErrorVerifyEvent;
            _Agent.ErrorMethodEvent -= _ErrorMethodEvent;
            _Connected = false;
			if(_BreakEvent != null)
			{
				_BreakEvent();
			}

			_BreakEvent = null;
			_Agent.Finial();
			_GhostRequest.SetPropertyDoneEvent -= _SoulProvider.SetPropertyDone;
			_GhostRequest.PingEvent -= _OnRequestPing;
			_GhostRequest.ReleaseEvent -= _SoulProvider.Unbind;
		}

		public INotifier<T> QueryProvider<T>()
		{
			return _Agent.QueryProvider<T>();
		}

		private void _Bind<TSoul>(TSoul soul)
		{
			_Binder.Bind(soul);
		}

		private void _Unbind<TSoul>(TSoul soul)
		{
			_Binder.Unbind(soul);
		}
	}
}
