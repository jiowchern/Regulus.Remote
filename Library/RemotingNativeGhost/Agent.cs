using System;
using System.Collections.Generic;
namespace Regulus.Remoting.Ghost.Native
{
    public partial class Agent : Regulus.Utility.IUpdatable, Regulus.Remoting.IGhostRequest, IAgent
    {
        class ConnectStage : Regulus.Game.IStage
        {
            private System.Net.Sockets.Socket _Socket;
            private string _Ipaddress;
            private int _Port;
            IAsyncResult _AsyncResult;

            public event Action<bool> ResultEvent;
            public ConnectStage(System.Net.Sockets.Socket socket, string ipaddress, int port)
            {
                // TODO: Complete member initialization
                this._Socket = socket;
                this._Ipaddress = ipaddress;
                this._Port = port;
            }            

            void Game.IStage.Enter()
            {                
                _AsyncResult = _Socket.BeginConnect(_Ipaddress, _Port, _ConnectResult, null);
            }

            private void _ConnectResult(IAsyncResult ar)
            {
                _Socket.EndConnect(ar);
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                if (_AsyncResult.IsCompleted)
                {
                    ResultEvent(true);
                }
            }
        }

        class IdleStage : Regulus.Game.IStage
        {

            void Game.IStage.Enter()
            {
                
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                
            }
        }
    }
    public partial class Agent : Regulus.Utility.IUpdatable, Regulus.Remoting.IGhostRequest, IAgent
	{
		Regulus.Remoting.AgentCore _Core;
        System.Net.Sockets.Socket _Socket;
		Queue<Package> _Sends;
        
		Regulus.Game.StageMachine _ReadMachine;
		Regulus.Game.StageMachine _WriteMachine;
        Regulus.Game.StageMachine _Machine;
        static System.Threading.ManualResetEvent _ManualResetEvent = new System.Threading.ManualResetEvent(false);
		public Agent()
		{
            _Core = new Remoting.AgentCore(this);
			_Sends = new Queue<Package>();		
			_ReadMachine = new Game.StageMachine();
			_WriteMachine = new Game.StageMachine();

            _Machine = new Game.StageMachine();
            _Socket = new System.Net.Sockets.Socket( System.Net.Sockets.AddressFamily.InterNetwork ,System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
		}
        public void Connect(string ipaddress, int port)
        {            
            _ToConnect(ipaddress, port);
            _ToIdle(_ReadMachine);
            _ToIdle(_WriteMachine);
        }

        private void _ToConnect(string ipaddress, int port)
        {
            var stage = new ConnectStage(_Socket, ipaddress, port);
            stage.ResultEvent += _ConnectResult;
            _Machine.Push(stage);
        }

        void _ConnectResult(bool success)
        {
            if (success == true)
            {
                _ToIdle(_Machine);
                _ToWrite();
                _ToRead();                
                _Core.Initial();
            }
        }

        private void _ToIdle(Regulus.Game.StageMachine machine)
        {
            machine.Push(new IdleStage());
        }
		bool Utility.IUpdatable.Update()
		{
            _Machine.Update();
			_ReadMachine.Update();
			_WriteMachine.Update();
			return true;
		}

		void Framework.ILaunched.Launch()
		{
            
		}

		

		private void _ToRead()
		{
            var stage = new NetworkStreamReadStage(_Socket);
			stage.ReadCompletionEvent += (package) =>
			{
				_Core.OnResponse(package.Code, package.Args);				
				_ToRead();
			};
			_ReadMachine.Push(stage);
		}

		private void _ToWrite()
		{
            var stage = new NetworkStreamWriteStage(_Socket, _Sends);
            stage.WriteCompletionEvent += _ToWrite;
			_WriteMachine.Push(stage);
		}

		void Framework.ILaunched.Shutdown()
		{            
			if (_Core != null)
				_Core.Finial();
		}

		void Remoting.IGhostRequest.Request(byte code, System.Collections.Generic.Dictionary<byte, byte[]> args)
		{
			_Sends.Enqueue(new Package() { Args = Regulus.Utility.Map<byte, byte[]>.ToMap(args), Code = code });
		}

		public long Ping { get { return _Core.Ping; } }

        IProviderNotice<T> IAgent.QueryProvider<T>()
        {
            return _Core.QueryProvider<T>();
        }
    }
}