using System;
using System.Collections.Generic;
namespace Regulus.Remoting.Ghost.Native
{
    
    public partial class Agent :  IAgent
	{        
        
        Regulus.Utility.StageMachine _Machine;
        
        event Action _BreakEvent;
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
            if (_ConnectValue != null && _ConnectValue.HasValue() == false)
                _ConnectValue.SetValue(false);
            _ConnectValue = new Regulus.Remoting.Value<bool>();
            var stage = new ConnectStage(ipaddress, port);
            stage.ResultEvent += (result,socket)=>
            {
                if (result && _ConnectEvent != null)
                    _ConnectEvent();

                _ConnectValue.SetValue(result);
                _ConnectResult(result, socket);                
            };
            _Machine.Push(stage);
            return _ConnectValue;
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
            onlineStage.DoneFromServerEvent += () =>
            {
                _Disconnect();
                if (_BreakEvent != null)
                    _BreakEvent();
            };

            onlineStage.DoneFromClientEvent += () =>
            {
                _Disconnect();
                if (_DisconnectionEvent != null)
                    _DisconnectionEvent();
            };
            
            machine.Push(onlineStage);
        }

        private void _Termination(Regulus.Utility.StageMachine machine)
        {
            _Machine.Termination();
            if (_ConnectValue != null && _ConnectValue.HasValue() == false)
                _ConnectValue.SetValue(false);
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

        event Action IAgent.BreakEvent
        {
            add { _BreakEvent += value; }
            remove { _BreakEvent -= value; }
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


        event Action _DisconnectionEvent;
        private Value<bool> _ConnectValue;
        event Action IAgent.DisconnectionEvent
        {
            add { _DisconnectionEvent += value; }
            remove { _DisconnectionEvent -= value; }
        }
    }
}