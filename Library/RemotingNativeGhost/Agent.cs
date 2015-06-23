using System;
using System.Collections.Generic;
namespace Regulus.Remoting.Ghost.Native
{
    
    public partial class Agent :  IAgent
	{        
        
        Regulus.Utility.StageMachine _Machine;
        bool _Connected;        
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
                _SendConnectResult();

                Regulus.Utility.Log.Instance.Write(string.Format("2.agent start connect"));
                _ConnectValue = new Regulus.Remoting.Value<bool>();
                var stage = new ConnectStage(ipaddress, port);
                stage.ResultEvent += (result, socket) =>
                {
                    Regulus.Utility.Log.Instance.Write(string.Format("3.connect result {0}", result));
                    _ConnectResult(result, socket);
                    _ConnectValue.SetValue(result);
                };
                _Machine.Push(stage);
                return _ConnectValue;
            }
            
        }
        
        void _ConnectResult(bool success , System.Net.Sockets.Socket socket)
        {
            if (success == true)
            {
                _Connected = true;
                if (_ConnectEvent != null)
                {                    
                    _ConnectEvent();
                }
                _ToOnline(_Machine, socket);                                
            }
            else
            {
                _Termination();  
            }
        }

        private void _ToOnline(Utility.StageMachine machine, System.Net.Sockets.Socket socket)
        {
            var onlineStage = new OnlineStage(socket, _Core);
            onlineStage.DoneFromServerEvent += () =>
            {
                _Termination();
                if (_BreakEvent != null)
                    _BreakEvent();
            };

            onlineStage.DoneFromClientEvent += () =>
            {
                _Termination();

            };
            Regulus.Utility.Log.Instance.Write(string.Format("4.agent start online"));
            machine.Push(onlineStage);
        }

        private void _Termination()
        {
            lock (_Machine)
            {
                Regulus.Utility.Log.Instance.Write(string.Format("agent start termination"));
                _Machine.Empty();
                _Connected = false;
                _SendConnectResult();
            }
        }

        private void _SendConnectResult()
        {
            if (_ConnectValue != null && _ConnectValue.HasValue() == false)
            {

                _ConnectValue.SetValue(false);
                Regulus.Utility.Log.Instance.Write(string.Format("agent sendConnectResult {0}" , false));
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

            _Termination();

            
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
            Regulus.Utility.Log.Instance.Write(string.Format("6.agent start disconnect"));
            _Termination();            
        }

        /// <summary>
        /// 建立代理器
        /// </summary>
        /// <returns></returns>
        public static IAgent Create()
        {
            return new Agent();
        }


        
        private Value<bool> _ConnectValue;        


        bool IAgent.Connected
        {
            get { return _Connected; }
        }
    }
}