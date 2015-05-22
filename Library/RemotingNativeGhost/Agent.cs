using System;
using System.Collections.Generic;
namespace Regulus.Remoting.Ghost.Native
{
    
    public partial class Agent :  IAgent
	{        
        
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
            var val = new Regulus.Remoting.Value<bool>();
            var stage = new ConnectStage(ipaddress, port);
            stage.ResultEvent += (result,socket)=>
            {
                if (_ConnectEvent != null)
                    _ConnectEvent();
                val.SetValue(result);
                _ConnectResult(result, socket);                
            };
            _Machine.Push(stage);
            return val;
        }
        
        void _ConnectResult(bool success , System.Net.Sockets.Socket socket)
        {
            if (success == true)
            {
                _ToOnline(_Machine, socket);                                
            }
            else
            {
                _ToOffline(_Machine);            
            }
        }

        private void _ToOnline(Utility.StageMachine machine, System.Net.Sockets.Socket socket)
        {
            _OnlineStage = new OnlineStage(socket);
            _OnlineStage.DoneEvent += () =>
            {
                _OnlineStage = null;

                //_ToOffline(_Machine);
                if (_DisconnectEvent != null)
                    _DisconnectEvent();
            };
            _Core.Finial();
            _Core.Initial(_OnlineStage);
            machine.Push(_OnlineStage);
        }

        private void _ToOffline(Regulus.Utility.StageMachine machine)
        {
            _Machine.Termination();            
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
            _Machine.Empty();
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