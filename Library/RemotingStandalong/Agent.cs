// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Agent.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Agent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;

using Regulus.Framework;
using Regulus.Utility;

#endregion

namespace Regulus.Remoting.Standalone
{
	public class Agent : IRequestQueue, IResponseQueue, ISoulBinder, IAgent
	{
		private event Action _BreakEvent;

		private event Action _ConnectEvent;

		public event ConnectedCallback ConnectedEvent;

		private readonly AgentCore _Agent;

		private readonly GhostRequest _GhostRequest;

		private readonly SoulProvider _SoulProvider;

		private bool _Connected;

		private ISoulBinder _Binder
		{
			get { return this._SoulProvider; }
		}

		public long Ping
		{
			get { return this._Agent.Ping; }
		}

		public Agent()
		{
			this._GhostRequest = new GhostRequest();
			this._Agent = new AgentCore();
			this._SoulProvider = new SoulProvider(this, this);
		}

		INotifier<T> IAgent.QueryNotifier<T>()
		{
			return this.QueryProvider<T>();
		}

		Value<bool> IAgent.Connect(string account, int password)
		{
			this.ConnectedEvent();
			this._Connected = true;
			return true;
		}

		long IAgent.Ping
		{
			get { return this._Agent.Ping; }
		}

		void IAgent.Disconnect()
		{
			this.Shutdown();
		}

		bool IUpdatable.Update()
		{
			this._Update();

			return true;
		}

		void IBootable.Launch()
		{
			this.Launch();
		}

		void IBootable.Shutdown()
		{
			this.Shutdown();
		}

		event Action IAgent.ConnectEvent
		{
			add { this._ConnectEvent += value; }
			remove { this._ConnectEvent -= value; }
		}

		event Action IAgent.BreakEvent
		{
			add { this._BreakEvent += value; }
			remove { this._BreakEvent -= value; }
		}

		bool IAgent.Connected
		{
			get { return this._Connected; }
		}

		event Action<Guid, string, Guid, byte[][]> IRequestQueue.InvokeMethodEvent
		{
			add { this._GhostRequest.CallMethodEvent += value; }
			remove { this._GhostRequest.CallMethodEvent -= value; }
		}

		event Action IRequestQueue.BreakEvent
		{
			add { this._BreakEvent += value; }
			remove { this._BreakEvent -= value; }
		}

		void IRequestQueue.Update()
		{
			this._Update();
		}

		void IResponseQueue.Push(byte cmd, Dictionary<byte, byte[]> args)
		{
			this._Agent.OnResponse(cmd, args);
		}

		void ISoulBinder.Return<TSoul>(TSoul soul)
		{
			this._Binder.Return(soul);
		}

		void ISoulBinder.Bind<TSoul>(TSoul soul)
		{
			this._Bind(soul);
		}

		void ISoulBinder.Unbind<TSoul>(TSoul soul)
		{
			this._Unbind(soul);
		}

		event Action ISoulBinder.BreakEvent
		{
			add { this._BreakEvent += value; }
			remove { this._BreakEvent -= value; }
		}

		public delegate void ConnectedCallback();

		public void Launch()
		{
			this._GhostRequest.PingEvent += this._OnRequestPing;
			this._GhostRequest.ReleaseEvent += this._SoulProvider.Unbind;

			this._Agent.Initial(this._GhostRequest);
		}

		private void _OnRequestPing()
		{
			this._Agent.OnResponse((byte)ServerToClientOpCode.Ping, null);
		}

		private void _Update()
		{
			this._SoulProvider.Update();
			this._GhostRequest.Update();
		}

		public void Shutdown()
		{
			this._Connected = false;
			if (this._BreakEvent != null)
			{
				this._BreakEvent();
			}

			this._BreakEvent = null;
			this._Agent.Finial();

			this._GhostRequest.PingEvent -= this._OnRequestPing;
			this._GhostRequest.ReleaseEvent -= this._SoulProvider.Unbind;
		}

		public INotifier<T> QueryProvider<T>()
		{
			return this._Agent.QueryProvider<T>();
		}

		private void _Bind<TSoul>(TSoul soul)
		{
			this._Binder.Bind(soul);
		}

		private void _Unbind<TSoul>(TSoul soul)
		{
			this._Binder.Unbind(soul);
		}
	}
}