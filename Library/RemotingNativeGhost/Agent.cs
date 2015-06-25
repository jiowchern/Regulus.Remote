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
            return _ToConnect(ipaddress, port);
        }

        private Regulus.Remoting.Value<bool> _ToConnect(string ipaddress, int port)
        {
            lock (_Machine)
            {                
                
                var connectValue = new Regulus.Remoting.Value<bool>();
                var stage = new ConnectStage(ipaddress, port);
                stage.ResultEvent += (result, socket) =>
                {
                    
                    _ConnectResult(result, socket);
                    connectValue.SetValue(result);
                };
                _Machine.Push(stage);
                return connectValue;
            }
            
        }
        
        void _ConnectResult(bool success , System.Net.Sockets.Socket socket)
        {
            if (success == true)
            {                
                if (_ConnectEvent != null)
                {                    
                    _ConnectEvent();
                }
                _ToOnline(_Machine, socket);                                
            }
            else
            {
                _ToTermination();  
            }
        }

        private void _ToOnline(Utility.StageMachine machine, System.Net.Sockets.Socket socket)
        {
            var onlineStage = new OnlineStage(socket, _Core);
            onlineStage.DoneFromServerEvent += () =>
            {
                _ToTermination();
                if (_BreakEvent != null)
                    _BreakEvent();
            };

            
            
            machine.Push(onlineStage);
        }

        private void _ToTermination()
        {
            lock (_Machine)
            {                
                
                _Machine.Push(new TerminationStage(this));
            }
        }        
        
		bool Utility.IUpdatable.Update()
		{
            lock (_Machine)
                _Machine.Update();            
			return true;
		}

		void Framework.IBootable.Launch()
		{
            
		}

		void Framework.IBootable.Shutdown()
		{

            if (_Core.Enable == true)
            {
                _ToTermination();

                while (_Core.Enable)
                {
                    lock (_Machine)
                    {
                        _Machine.Update();
                    }
                }
            }
            
		}

		

		long _Ping { get 
        {
            return _Core.Ping;
        } }

        INotifier<T> IAgent.QueryNotifier<T>()
        {
            return _Core.QueryProvider<T>();
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
            _ToTermination();            
        }

        /// <summary>
        /// 建立代理器
        /// </summary>
        /// <returns></returns>
        public static IAgent Create()
        {
            return new Agent();
        }


        bool IAgent.Connected
        {
            get { return _Core.Enable; }
        }
    }
}