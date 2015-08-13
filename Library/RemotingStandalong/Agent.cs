using System;
using System.Collections.Generic;


using Regulus.Framework;
using Regulus.Utility;

<<<<<<< HEAD
namespace Regulus.Remoting.Standalong
=======
#endregion

namespace Regulus.Remoting.Standalone
>>>>>>> bb08c0b8a8aa5ec0c708cd9f624c302cd192eb5d
{
	public class Agent : IRequestQueue, IResponseQueue, ISoulBinder, IAgent
	{
		public delegate void ConnectedCallback();

		private event Action _BreakEvent;

		private event Action _ConnectEvent;

		public event ConnectedCallback ConnectedEvent;

		private readonly AgentCore _Agent;

		private readonly GhostRequest _GhostRequest;

		private readonly SoulProvider _SoulProvider;

		private bool _Connected;

		private ISoulBinder _Binder
		{
			get { return _SoulProvider; }
		}

		public long Ping
		{
			get { return _Agent.Ping; }
		}

		public Agent()
		{
			_GhostRequest = new GhostRequest();
			_Agent = new AgentCore();
			_SoulProvider = new SoulProvider(this, this);
		}

		INotifier<T> IAgent.QueryNotifier<T>()
		{
			return QueryProvider<T>();
		}

		Value<bool> IAgent.Connect(string account, int password)
		{
			ConnectedEvent();
			_Connected = true;
			return true;
		}

		long IAgent.Ping
		{
			get { return _Agent.Ping; }
		}

		void IAgent.Disconnect()
		{
			Shutdown();
		}

		bool IUpdatable.Update()
		{
			_Update();

			return true;
		}

		void IBootable.Launch()
		{
			Launch();
		}

		void IBootable.Shutdown()
		{
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

		event Action<Guid, string, Guid, byte[][]> IRequestQueue.InvokeMethodEvent
		{
			add { _GhostRequest.CallMethodEvent += value; }
			remove { _GhostRequest.CallMethodEvent -= value; }
		}

		event Action IRequestQueue.BreakEvent
		{
			add { _BreakEvent += value; }
			remove { _BreakEvent -= value; }
		}

		void IRequestQueue.Update()
		{
			_Update();
		}

		void IResponseQueue.Push(byte cmd, Dictionary<byte, byte[]> args)
		{
			_Agent.OnResponse(cmd, args);
		}

		void ISoulBinder.Return<TSoul>(TSoul soul)
		{
			_Binder.Return(soul);
		}

		void ISoulBinder.Bind<TSoul>(TSoul soul)
		{
			_Bind(soul);
		}

		void ISoulBinder.Unbind<TSoul>(TSoul soul)
		{
			_Unbind(soul);
		}

		event Action ISoulBinder.BreakEvent
		{
			add { _BreakEvent += value; }
			remove { _BreakEvent -= value; }
		}

		public void Launch()
		{
			_GhostRequest.PingEvent += _OnRequestPing;
			_GhostRequest.ReleaseEvent += _SoulProvider.Unbind;

			_Agent.Initial(_GhostRequest);
		}

		private void _OnRequestPing()
		{
			_Agent.OnResponse((byte)ServerToClientOpCode.Ping, null);
		}

		private void _Update()
		{
			_SoulProvider.Update();
			_GhostRequest.Update();
		}

		public void Shutdown()
		{
			_Connected = false;
			if(_BreakEvent != null)
			{
				_BreakEvent();
			}

			_BreakEvent = null;
			_Agent.Finial();

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
