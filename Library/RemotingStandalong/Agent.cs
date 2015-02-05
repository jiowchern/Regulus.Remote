using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Standalong
{
	public class Agent : Regulus.Remoting.IRequestQueue, Regulus.Remoting.IResponseQueue, Regulus.Remoting.ISoulBinder , Regulus.Remoting.IAgent
	{

        public delegate void ConnectedCallback();
        public event ConnectedCallback ConnectedEvent;
		Regulus.Remoting.AgentCore	_Agent;	
		Regulus.Remoting.Soul.SoulProvider	_SoulProvider;
		GhostRequest	_GhostRequest;
        public Agent()
        {
            _GhostRequest = new GhostRequest();
            _Agent = new Remoting.AgentCore(_GhostRequest);
            _SoulProvider = new Remoting.Soul.SoulProvider(this, this);
        }
		public void Launch()
		{
			
			_GhostRequest.PingEvent	+= _OnRequestPing	;						
			
			
		}
		private void _OnRequestPing()
		{
			_Agent.OnResponse( (byte)Regulus.Remoting.ServerToClientOpCode.Ping , null ) ;
		}		

		public void Update()
		{
			_SoulProvider.Update();
			_GhostRequest.Update();
		}

		public void Shutdown()
		{
            if (BreakEvent != null)
                BreakEvent();
            BreakEvent = null;
			_Agent.Finial();
			_SoulProvider = null;
			_GhostRequest.PingEvent -= _OnRequestPing;			
		}

		event Action<Guid, string, Guid, byte[][]> Remoting.IRequestQueue.InvokeMethodEvent
		{
			add { _GhostRequest.CallMethodEvent += value ; }
			remove { _GhostRequest.CallMethodEvent -= value; }
		}

		
		
		

		void Remoting.IResponseQueue.Push(byte cmd, Dictionary<byte, byte[]> args)
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

        public void Disconnect()
        {
            Shutdown();
        }

        Remoting.Ghost.IProviderNotice<T> Remoting.IAgent.QueryProvider<T>()
        {
            return QueryProvider<T>();
        }

        Remoting.Value<bool> Remoting.IAgent.Connect(string account, int password)
        {
            ConnectedEvent();
            return true;
        }


        long Remoting.IAgent.Ping
        {
            get { return _Agent.Ping; }
        }

        event Action _DisconnectEvent;
        event Action Remoting.IAgent.DisconnectEvent
        {
            add { _DisconnectEvent += value; }
            remove { _DisconnectEvent -= value; }
        }

        void Remoting.IAgent.Disconnect()
        {
            if (_DisconnectEvent != null)
                _DisconnectEvent();
        }

        bool Utility.IUpdatable.Update()
        {
            Update();

            return true;
        }

        void Framework.ILaunched.Launch()
        {
            Launch();
        }

        void Framework.ILaunched.Shutdown()
        {
            Shutdown();
        }







        event Action _ConnectEvent;
        event Action Remoting.IAgent.ConnectEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }
}
