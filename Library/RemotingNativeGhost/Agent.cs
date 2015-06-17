using System;
using System.Collections.Generic;
namespace Regulus.Remoting.Ghost.Native
{
    
    public partial class Agent :  IAgent
	{        
        
        Regulus.Utility.StageMachine _Machine;
        
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
                if (result && _ConnectEvent != null)
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
                _Disconnect();  
            }
        }

        private void _ToOnline(Utility.StageMachine machine, System.Net.Sockets.Socket socket)
        {
            var onlineStage = new OnlineStage(socket, _Core);
            onlineStage.DoneEvent += () =>
            {
                if (_DisconnectEvent != null)
                    _DisconnectEvent();

                _Disconnect();
            };
            
            machine.Push(onlineStage);
        }

        private void _Termination(Regulus.Utility.StageMachine machine)
        {
            _Machine.Termination();
        }

        
		bool Utility.IUpdatable.Update()
		{
            _Machine.Update();            
            

			return true;
		}

		void Framework.IBootable.Launch()
		{
            
		}

		void Framework.IBootable.Shutdown()
		{
            
            _Disconnect();
		}

		

		long _Ping { get 
        {
            return _Core.Ping;
        } }

        INotifier<T> IAgent.QueryNotifier<T>()
        {
            return _Core.QueryProvider<T>();
        }

        void _Disconnect()
        {
            _Termination(_Machine);
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
            _Termination(_Machine);
        }

        /// <summary>
        /// 建立代理器
        /// </summary>
        /// <returns></returns>
        public static IAgent Create()
        {
            return new Agent();
        }
    }
}