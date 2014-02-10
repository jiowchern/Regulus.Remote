namespace Regulus.Remoting
{
    public partial class NetworkStreamWriteStage : Regulus.Game.IStage
    {
        class WrittingStage : Regulus.Game.IStage
        {
            System.Net.Sockets.Socket _Socket;
            System.IAsyncResult _AsyncResult;
            byte[] _Buffer;
            public event System.Action DoneEvent;
            

            public WrittingStage(System.Net.Sockets.Socket socket, byte[] buffer)
            {                
                this._Socket = socket;
                _Buffer = buffer;
            }
            void Game.IStage.Enter()            
            {                
                _AsyncResult = _Socket.BeginSend(_Buffer, 0, _Buffer.Length, 0, _WriteCompletion, null);
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                if (_AsyncResult.IsCompleted)
                {
                    DoneEvent();
                }
            }

            private void _WriteCompletion(System.IAsyncResult ar)
            {
                _Socket.EndSend(ar);
            }
        }
    }

	public partial class NetworkStreamWriteStage : Regulus.Game.IStage
	{
		System.Net.Sockets.Socket _Socket;
		System.Collections.Generic.Queue<Package> _Packages;
        
        public event System.Action WriteCompletionEvent;
        Regulus.Game.StageMachine _Machine;        

        public NetworkStreamWriteStage(System.Net.Sockets.Socket socket, System.Collections.Generic.Queue<Package> packages)
		{
            _Socket = socket;
			_Packages = packages;
            _Machine = new Game.StageMachine();
		}
		void Game.IStage.Enter()
		{
			if (_Packages.Count > 0)
			{
				var package = _Packages.Dequeue();
                if (package == null)
                {
                    throw new System.NullReferenceException();
                }
				var buffer = Regulus.PhotonExtension.TypeHelper.Serializer(package);
                _ToHead(buffer);                
			}
			else
			{
				WriteCompletionEvent();
			}
		}

        private void _ToHead(byte[] buffer)
        {
            var header = System.BitConverter.GetBytes((int)buffer.Length);
            var stage = new WrittingStage(_Socket, header);
            stage.DoneEvent += () => { _ToBody(buffer); };
            _Machine.Push(stage);
        }

        private void _ToBody(byte[] buffer)
        {            
            var stage = new WrittingStage(_Socket, buffer);
            stage.DoneEvent += WriteCompletionEvent;
            _Machine.Push(stage);
        }

		void Game.IStage.Leave()
		{

		}

		void Game.IStage.Update()
		{
            try
            {
                _Machine.Update();
            }
            catch (System.Net.Sockets.SocketException ex)
            { 

            }
            
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
            bool _Done;
            public ReadingStage(System.Net.Sockets.Socket socket, int size)
            {                
                this._Socket = socket;
                this._Size = size;
                _Buffer = new byte[size];
                _Done = false;
            }


            void Game.IStage.Enter()
            {
                _Done = false;
                _Socket.BeginReceive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);                
            }

            void Game.IStage.Leave()
            {
                
            }

            void Game.IStage.Update()
            {
                if (_Done)
                { 
                    DoneEvent(_Buffer);                    
                }
            }

            private void _Readed(System.IAsyncResult ar)
            {
                try
                {
                    _Offset += _Socket.EndReceive(ar);
                    if (_Offset == _Size)
                    {
                        _Done = true;
                    }
                    else
                    {
                        _Done = false;
                        _Socket.BeginReceive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);
                    }                
                }
                catch (System.Net.Sockets.SocketException ex)
                { 

                }
                
            }

        }
    }
    
	public partial class NetworkStreamReadStage : Regulus.Game.IStage
	{
        public delegate void OnReadCompletion(Package package);
        public event OnReadCompletion ReadCompletionEvent;
		System.Net.Sockets.Socket _Socket;
        Regulus.Game.StageMachine _Machine;
        const int _HeadSize = 4;
        public NetworkStreamReadStage(System.Net.Sockets.Socket socket )
		{            
			_Socket = socket;
            _Machine = new Game.StageMachine();            
		}
		void Game.IStage.Enter()
		{
            _ToHead();
		}

        private void _ToHead()
        {
            var stage = new ReadingStage(_Socket, _HeadSize);
            stage.DoneEvent+= _ToBody;            
            _Machine.Push(stage);
        }
        private void _ToBody(byte[] head)
        {           
            var bodySize = System.BitConverter.ToInt32(head , 0);
            var stage = new ReadingStage(_Socket, bodySize);
            stage.DoneEvent += _Done;
            _Machine.Push(stage);
        }
        private void _Done(byte[] body)
        {
            ReadCompletionEvent(Regulus.PhotonExtension.TypeHelper.Deserialize(body) as Package);
        }

		void Game.IStage.Leave()
		{

		}
        
		void Game.IStage.Update()
		{
            try
            { 
                _Machine.Update();
            }
            catch(System.Net.Sockets.SocketException ex)
            {

            }
            
		}
	}
}