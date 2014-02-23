using System;
using System.Collections.Generic;
namespace Regulus.Remoting.Ghost.Native
{
    
    public partial class Agent : Regulus.Utility.IUpdatable,  IAgent
	{        
        System.Net.Sockets.Socket _Socket;		
        Regulus.Game.StageMachine _Machine;
        OnlineStage _OnlineStage;
        public event Action DisconnectEvent;
        
		public Agent()
		{            
            _Machine = new Game.StageMachine();
            _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            _Socket.NoDelay = true;
            _Socket.Blocking = false;

            _OnlineStage = new OnlineStage();
            _OnlineStage.DoneEvent += () =>
            {
                _ToOffline(_Machine);
                if(DisconnectEvent != null)
                    DisconnectEvent();
            };
		}
        
        public Regulus.Remoting.Value<bool> Connect(string ipaddress, int port)
        {
            Disconnect();
            return _ToConnect(ipaddress, port);
        }

        private Regulus.Remoting.Value<bool> _ToConnect(string ipaddress, int port)
        {            
            _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            _Socket.NoDelay = true;
            _Socket.Blocking = false;
            
            var val = new Regulus.Remoting.Value<bool>();
            var stage = new ConnectStage(_Socket, ipaddress, port);
            stage.ResultEvent += (result)=>
            {
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

        private void _ToOnline(Game.StageMachine machine)
        {            
            var stage = _OnlineStage;
            stage.SetSocket(_Socket);
            machine.Push(_OnlineStage);
        }

        private void _ToOffline(Regulus.Game.StageMachine machine)
        {
            
            machine.Push(new IdleStage());
        }

        
		bool Utility.IUpdatable.Update()
		{
            _Machine.Update();
            _OnlineStage.Process();
            

			return true;
		}

		void Framework.ILaunched.Launch()
		{
            
		}

		void Framework.ILaunched.Shutdown()
		{
            Disconnect();
		}

		

		public long Ping { get { return _OnlineStage.Ping ; } }

        IProviderNotice<T> IAgent.QueryProvider<T>()
        {
            return _OnlineStage.QueryProvider<T>();
        }

        public void Disconnect()
        {
            if (_Socket.Connected)
                _Socket.Close();                        
        }
    }
}