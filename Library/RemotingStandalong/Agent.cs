using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Standalong
{
	public class Agent : Regulus.Remoting.IRequestQueue, Regulus.Remoting.IResponseQueue, Regulus.Remoting.ISoulBinder
	{
		Regulus.Remoting.AgentCore	_Agent;	
		Regulus.Remoting.Soul.SoulProvider	_SoulProvider;
		GhostRequest	_GhostRequest;

		public void Launch()
		{
			_GhostRequest = new GhostRequest();
			_GhostRequest.PingEvent	+= _OnRequestPing	;			
			_Agent = new Remoting.AgentCore(_GhostRequest);
			_SoulProvider = new Remoting.Soul.SoulProvider( this , null);
			
		}
		private void _OnRequestPing()
		{
			_Agent.OnResponse( (byte)Regulus.Remoting.ServerToClientPhotonOpCode.Ping , null ) ;
		}		

		public void Update()
		{
			_SoulProvider.Update();
			_GhostRequest.Update();
		}

		public void Shutdown()
		{
			_Agent.Finial();
			_SoulProvider = null;
			_GhostRequest.PingEvent -= _OnRequestPing;			
		}

		event Action<Guid, string, Guid, object[]> Remoting.IRequestQueue.InvokeMethodEvent
		{
			add { _GhostRequest.CallMethodEvent += value ; }
			remove { _GhostRequest.CallMethodEvent -= value; }
		}

		
		
		void Remoting.IRequestQueue.Update()
		{
			
		}

		void Remoting.IResponseQueue.Push(byte cmd, Dictionary<byte, object> args)
		{
			_Agent.OnResponse(cmd , args);
		}


		public long Ping
		{
			get { return _Agent.Ping; }
		}

		public Regulus.Remoting.Ghost.IProviderNotice<T> QueryProvider<T>()
		{
			return _Agent.QueryProvider<T>();
		}


		public event Action BreakEvent;

		public void Bind<TSoul>(TSoul soul)
		{
			_SoulProvider.Bind<TSoul>(soul);
		}
		public void Unbind<TSoul>(TSoul soul)
		{
			_SoulProvider.Unbind<TSoul>(soul);
		}
	}
}
