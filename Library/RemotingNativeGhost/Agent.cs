using System;
using System.Collections.Generic;
namespace Regulus.Remoting.Ghost.Native
{
    
    public partial class Agent :  IAgent
	{        
        System.Net.Sockets.Socket _Socket;		
        Regulus.Utility.StageMachine _Machine;
        OnlineStage _OnlineStage;
        event Action _DisconnectEvent;
        AgentCore _Core;
		private Agent()
		{            
            _Machine = new Utility.StageMachine();
            _Core = new AgentCore();
		}
        
        Regulus.Remoting.Value<bool> _Connect(string ipaddress, int port)
        {
            _Disconnect();
            return _ToConnect(ipaddress, port);
        }

        private Regulus.Remoting.Value<bool> _ToConnect(string ipaddress, int port)
        {            
            _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            _Socket.NoDelay = true;
            
            
            var val = new Regulus.Remoting.Value<bool>();
            var stage = new ConnectStage(_Socket, ipaddress, port);
            stage.ResultEvent += (result)=>
            {
                if (_ConnectEvent != null)
                    _ConnectEvent();
                val.SetValue(result);
                _ConnectResult(result);                
            };
            _Machine.Push(stage);
            return val;
        }
        
        void _ConnectResult(bool success)
        {
            if (success == true)
            {
                _ToOnline(_Machine);                                
            }
            else
            {
                _ToOffline(_Machine);            
            }
        }

        private void _ToOnline(Utility.StageMachine machine)
        {
            _OnlineStage = new OnlineStage(_Socket);
            _OnlineStage.DoneEvent += () =>
            {
                _OnlineStage = null;
                if (_DisconnectEvent != null)
                    _DisconnectEvent();
                _ToOffline(_Machine);
            };
            _Core.Finial();
            _Core.Initial(_OnlineStage);
            machine.Push(_OnlineStage);
        }

        private void _ToOffline(Regulus.Utility.StageMachine machine)
        {
            
            machine.Push(new IdleStage());
        }

        
		bool Utility.IUpdatable.Update()
		{
            _Machine.Update();
            if (_OnlineStage != null)
                _OnlineStage.Process(_Core);
            

			return true;
		}

		void Framework.ILaunched.Launch()
		{
            
		}

		void Framework.ILaunched.Shutdown()
		{
            
            _Disconnect();
		}

		

		long _Ping { get 
        {
            return _Core.Ping;
        } }

        IProviderNotice<T> IAgent.QueryProvider<T>()
        {
            return _Core.QueryProvider<T>();
        }

        void _Disconnect()
        {            
            _Machine.Termination();
            if (_Socket != null)
            {                
                _Socket.Close();
                _Socket = null;
            }
        }


        Value<bool> IAgent.Connect(string account, int password)
        {
            return _Connect(account, password);
        }

        event Action _ConnectEvent;
        event Action IAgent.ConnectEvent
        {
            add { _ConnectEvent += value; }
            remove { _ConnectEvent -= value; }
        }

        long IAgent.Ping
        {
            get { return _Ping; }
        }

        event Action IAgent.DisconnectEvent
        {
            add { _DisconnectEvent += value; }
            remove { _DisconnectEvent -= value; }
        }

        void IAgent.Disconnect()
        {
            _Disconnect();
        }


        public static IAgent Create()
        {
            return new Agent();
        }
    }
}