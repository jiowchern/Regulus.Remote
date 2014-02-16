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
            bool _Result;
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
                try
                {
                    _Socket.EndConnect(ar);
                    _Result = true;
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    _Result = false;
                }
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                if (_AsyncResult.IsCompleted)
                {
                    ResultEvent(_Result);
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
        Queue<Package> _Receives;

        System.Threading.Thread _IOThread;
        volatile bool _ThreadRun;
        
		Regulus.Game.StageMachine _ReadMachine;
		Regulus.Game.StageMachine _WriteMachine;
        Regulus.Game.StageMachine _Machine;
        
		public Agent()
		{
            _Core = new Remoting.AgentCore(this);
			_Sends = new Queue<Package>();
            _Receives = new Queue<Package>();
			_ReadMachine = new Game.StageMachine();
			_WriteMachine = new Game.StageMachine();            
            _Machine = new Game.StageMachine();
            _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
		}

        
        public Regulus.Remoting.Value<bool> Connect(string ipaddress, int port)
        {

            Disconnect();
            return _ToConnect(ipaddress, port);
        }

        private Regulus.Remoting.Value<bool> _ToConnect(string ipaddress, int port)
        {            
            _Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
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
        void _HandleIO()
        {
            _ToWrite();
            _ToRead();

            while (_Socket.Connected && _ThreadRun )
            {
                _ReadMachine.Update();
                _WriteMachine.Update();
            }

            _ToIdle(_ReadMachine);
            _ToIdle(_WriteMachine);
        }
        void _ConnectResult(bool success)
        {
            if (success == true)
            {
                _ToIdle(_Machine);                
                _Core.Initial();
                _ThreadRun = true;
                _IOThread = new System.Threading.Thread(_HandleIO);
                _IOThread.Start();
            }
            else
            {
                _ToIdle(_Machine);            
            }
        }

        private void _ToIdle(Regulus.Game.StageMachine machine)
        {
            machine.Push(new IdleStage());
        }

        public bool Connected { get { return _Socket.Connected; } }
		bool Utility.IUpdatable.Update()
		{
            _Machine.Update();
            while (_Receives.Count > 0)
            {
                lock (_Receives)
                {
                    var package = _Receives.Dequeue();
                    _Core.OnResponse(package.Code, package.Args);				
                }
            }            
            
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
                lock (_Receives)
                {
                    _Receives.Enqueue(package);
                }
				_ToRead();
			};
			_ReadMachine.Push(stage);
		}

		private void _ToWrite()
		{
            lock (_Sends)
            {
                if (_Sends.Count > 0)
                {
                    var package = _Sends.Dequeue();
                    var stage = new NetworkStreamWriteStage(_Socket, package);
                    stage.WriteCompletionEvent += _ToWrite;                    
                    _WriteMachine.Push(stage);
                }
                else
                {
                    var stage = new WaitQueueStage(_Sends);
                    stage.DoneEvent += _ToWrite;
                    _WriteMachine.Push(stage);
                }
            }            
		}

		void Framework.ILaunched.Shutdown()
		{
            Disconnect();
		}

		void Remoting.IGhostRequest.Request(byte code, System.Collections.Generic.Dictionary<byte, byte[]> args)
		{
            lock (_Sends)
            {
                _Sends.Enqueue(new Package() { Args = Regulus.Utility.Map<byte, byte[]>.ToMap(args), Code = code });
            }			
		}

		public long Ping { get { return _Core.Ping; } }

        IProviderNotice<T> IAgent.QueryProvider<T>()
        {
            return _Core.QueryProvider<T>();
        }

        public void Disconnect()
        {
            _ThreadRun = false;
            if (_IOThread != null &&_IOThread.IsAlive)
            {
                _IOThread.Abort();
                _IOThread.Join();
            }
            _IOThread = null;
            
            if (_Socket.Connected)
                _Socket.Close();
            if (_Core != null)
                _Core.Finial();
            
        }
    }
}