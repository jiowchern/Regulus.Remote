namespace Regulus.Remoting
{
    public enum SocketIOResult
    {
        None, Done, Break
    }
    public partial class NetworkStreamWriteStage : Regulus.Game.IStage
    {        
        class WrittingStage : Regulus.Game.IStage
        {
            System.Net.Sockets.Socket _Socket;
            System.IAsyncResult _AsyncResult;
            byte[] _Buffer;
            public event System.Action<SocketIOResult> DoneEvent;            
            SocketIOResult _Result;
            public WrittingStage(System.Net.Sockets.Socket socket, byte[] buffer)
            {                
                this._Socket = socket;
                _Buffer = buffer;
                
            }
            void Game.IStage.Enter()            
            {
                try
                {
                    _Result = SocketIOResult.None;
                    _AsyncResult = _Socket.BeginSend(_Buffer, 0, _Buffer.Length, 0, _WriteCompletion, null);
                }
                catch 
                {
                    _Result = SocketIOResult.Break;
                }
                    
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                if (_Result != SocketIOResult.None)
                {
                    DoneEvent(_Result);
                }
            }

            private void _WriteCompletion(System.IAsyncResult ar)
            {
                try                
                {
                    _Socket.EndSend(ar);
                    _Result = SocketIOResult.Done;
                }
                catch 
                {
                    _Result = SocketIOResult.Break;
                }                
            }
        }
    }

	public partial class NetworkStreamWriteStage : Regulus.Game.IStage
	{
		System.Net.Sockets.Socket _Socket;
		Package _Package;
        
        public event System.Action WriteCompletionEvent;
        public event System.Action ErrorEvent;
        Regulus.Game.StageMachine _Machine;        

        public NetworkStreamWriteStage(System.Net.Sockets.Socket socket, Package package)
		{
            _Socket = socket;
            _Package = package;
            _Machine = new Game.StageMachine();
		}
		void Game.IStage.Enter()
		{

            var package = _Package;            
			var buffer = Regulus.PhotonExtension.TypeHelper.Serializer(package);
            _ToHead(buffer);                
			
		}

        private void _ToHead(byte[] buffer)
        {
            var header = System.BitConverter.GetBytes((int)buffer.Length);
            var stage = new WrittingStage(_Socket, header);
            stage.DoneEvent += (result) => 
            {
                if (result == SocketIOResult.Done)
                    _ToBody(buffer);
                else
                    _ToHead(buffer);
            };
            
            _Machine.Push(stage);
        }

        private void _ToBody(byte[] buffer)
        {            
            var stage = new WrittingStage(_Socket, buffer);
            stage.DoneEvent += (result) => 
            {
                if (result == SocketIOResult.Done)
                    WriteCompletionEvent();
                else
                    _ToBody(buffer);
            };
            
            _Machine.Push(stage);
        }

		void Game.IStage.Leave()
		{

		}

		void Game.IStage.Update()
		{
            
            _Machine.Update();
            
            
		}
	}


    public partial class NetworkStreamReadStage : Regulus.Game.IStage
    {
        
        class ReadingStage : Regulus.Game.IStage
        {
            public event System.Action<byte[]> DoneEvent;            
            private System.Net.Sockets.Socket _Socket;
            private int _Size;
            int _Offset;
            byte[] _Buffer;
            SocketIOResult _Result;
            public ReadingStage(System.Net.Sockets.Socket socket, int size)
            {                
                this._Socket = socket;
                this._Size = size;
            }


            void Game.IStage.Enter()
            {

                try
                {
                    _Buffer = new byte[_Size];                
                    _Result = SocketIOResult.None;
                    _Socket.BeginReceive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);
                }
                catch 
                {
                    _Result = SocketIOResult.Break;
                }
                
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                if (_Result == SocketIOResult.Done)
                { 
                    DoneEvent(_Buffer);                    
                }
                else if (_Result == SocketIOResult.Break)
                    DoneEvent(null);                    
            }

            private void _Readed(System.IAsyncResult ar)
            {
                try
                {
                    _Offset += _Socket.EndReceive(ar);
                    if (_Offset == _Size)
                    {
                        _Result = SocketIOResult.Done;
                    }
                    else
                    {
                        _Result = SocketIOResult.None;
                        _Socket.BeginReceive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);
                    }                
                }
                catch 
                {
                    _Result = SocketIOResult.Break; 
                }
                
            }

        }
    }
    
	public partial class NetworkStreamReadStage : Regulus.Game.IStage
	{
        public delegate void OnReadCompletion(Package package);
        public event OnReadCompletion ReadCompletionEvent;
        public event System.Action ErrorEvent;       
		System.Net.Sockets.Socket _Socket;
        Regulus.Game.StageMachine _Machine;
        const int _HeadSize = 4;
        public NetworkStreamReadStage(System.Net.Sockets.Socket socket )
		{            
			_Socket = socket;
            _Machine = new Game.StageMachine();
            ErrorEvent = () => { };
		}
		void Game.IStage.Enter()
		{
            _ToHead();
		}

        private void _ToHead()
        {
            var stage = new ReadingStage(_Socket, _HeadSize);
            stage.DoneEvent += (buffer)=>
            {
                if (buffer != null)
                    _ToBody(buffer);
                else
                    ErrorEvent();
            };
            
            _Machine.Push(stage);
        }
        private void _ToBody(byte[] head)
        {           
            var bodySize = System.BitConverter.ToInt32(head , 0);
            var stage = new ReadingStage(_Socket, bodySize);
            stage.DoneEvent += (buffer)=>
            {
                if (buffer != null)
                    _Done(buffer);
                else
                    ErrorEvent();
            };
            
            _Machine.Push(stage);
        }
        private void _Done(byte[] body)
        {
            ReadCompletionEvent(Regulus.PhotonExtension.TypeHelper.Deserialize<Package>(body) );
        }

		void Game.IStage.Leave()
		{

		}
        
		void Game.IStage.Update()
		{            
            _Machine.Update();
		}
	}

    public partial class WaitQueueStage : Regulus.Game.IStage
    {


        public event System.Action DoneEvent;
        private System.Collections.Generic.Queue<Package> _Packages;

        public WaitQueueStage(System.Collections.Generic.Queue<Package> packages)
        {            
            this._Packages = packages;
        }
        void Game.IStage.Enter()
        {
            
        }

        void Game.IStage.Leave()
        {
            
        }

        void Game.IStage.Update()
        {
            if (_Packages.Count > 0)
            {
                DoneEvent();
            }
        }
    }
}