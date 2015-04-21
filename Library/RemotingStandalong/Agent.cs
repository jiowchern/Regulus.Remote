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
        Regulus.Remoting.ISoulBinder _Binder { get { return _SoulProvider; } }
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
            _GhostRequest.ReleaseEvent += _SoulProvider.Unbind;
			
			
		}
		private void _OnRequestPing()
		{
			_Agent.OnResponse( (byte)Regulus.Remoting.ServerToClientOpCode.Ping , null ) ;
		}		

		private void _Update()
		{
			_SoulProvider.Update();
			_GhostRequest.Update();
		}

		public void Shutdown()
		{
            if (_BreakEvent != null)
                _BreakEvent();
            _BreakEvent = null;
			_Agent.Finial();
			
			_GhostRequest.PingEvent -= _OnRequestPing;
            _GhostRequest.ReleaseEvent -= _SoulProvider.Unbind;

            _SoulProvider = null;
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


        private event Action _BreakEvent;

		private void _Bind<TSoul>(TSoul soul)
		{
            _Binder.Bind<TSoul>(soul);
		}
        private void _Unbind<TSoul>(TSoul soul)
		{
            _Binder.Unbind<TSoul>(soul);
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
            _Update();

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
            add { _ConnectEvent += value; }
            remove { _ConnectEvent -= value; }
        }

        void Remoting.ISoulBinder.Return<TSoul>(TSoul soul)
        {
            _Binder.Return(soul);
        }

        void Remoting.ISoulBinder.Bind<TSoul>(TSoul soul)
        {
            _Bind(soul);
        }

        void Remoting.ISoulBinder.Unbind<TSoul>(TSoul soul)
        {
            _Unbind(soul);
        }

        event Action Remoting.ISoulBinder.BreakEvent
        {
            add { _BreakEvent += value; }
            remove { _BreakEvent -= value; }
        }


        event Action Remoting.IRequestQueue.BreakEvent
        {
            add { _BreakEvent += value; }
            remove { _BreakEvent -= value; }
        }

        void Remoting.IRequestQueue.Update()
        {
            _Update();
        }
    }
}
