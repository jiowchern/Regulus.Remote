namespace Regulus.Remoting
{
	public class NetworkStreamWriteStage : Regulus.Game.IStage
	{
		System.Net.Sockets.Socket _Socket;
		System.Collections.Generic.Queue<Package> _Packages;
        System.IAsyncResult _AsyncResult;
        public NetworkStreamWriteStage(System.Net.Sockets.Socket socket, System.Collections.Generic.Queue<Package> packages)
		{
            _Socket = socket;
			_Packages = packages;
		}
		void Game.IStage.Enter()
		{
			if (_Packages.Count > 0)
			{
				var package = _Packages.Dequeue();
				var buffer = Regulus.PhotonExtension.TypeHelper.Serializer(package);
                _AsyncResult = _Socket.BeginSend(buffer, 0, buffer.Length, 0, _WriteCompletion, null);
			}
			else
			{
				WriteCompletionEvent();
			}
		}

		public delegate void OnWriteCompletion();
		public event OnWriteCompletion WriteCompletionEvent;
		private void _WriteCompletion(System.IAsyncResult ar)
		{
            _Socket.EndSend(ar);			
		}

		void Game.IStage.Leave()
		{

		}

		void Game.IStage.Update()
		{
            if (_AsyncResult != null && _AsyncResult.IsCompleted)
            {
                WriteCompletionEvent();
            }
		}
	}
	public class NetworkStreamReadStage : Regulus.Game.IStage
	{
		System.Net.Sockets.Socket _Socket;
		byte[] _Buffer;
        System.IAsyncResult _AsyncResult;
        public NetworkStreamReadStage(System.Net.Sockets.Socket socket, int size)
		{
			_Buffer = new byte[size];
			_Socket = socket;
		}
		void Game.IStage.Enter()
		{
            _AsyncResult = _Socket.BeginReceive(_Buffer, 0, _Buffer.Length, 0, _Readed, null);
		}

		public delegate void OnReadCompletion(Package package);
		public event OnReadCompletion ReadCompletionEvent;

		private void _Readed(System.IAsyncResult ar)
		{
            _Socket.EndReceive(ar);            
		}

		void Game.IStage.Leave()
		{

		}
        
		void Game.IStage.Update()
		{
            if (_AsyncResult.IsCompleted)
            {
                ReadCompletionEvent(Regulus.PhotonExtension.TypeHelper.Deserialize(_Buffer) as Package);
            }
		}
	}
}