namespace Regulus.Remoting
{
	public class NetworkStreamWriteStage : Regulus.Game.IStage
	{
		System.Net.Sockets.NetworkStream _Stream;
		System.Collections.Generic.Queue<Package> _Packages;
		public NetworkStreamWriteStage(System.Net.Sockets.NetworkStream stream, System.Collections.Generic.Queue<Package> packages)
		{
			_Stream = stream;
			_Packages = packages;
		}
		void Game.IStage.Enter()
		{
			if (_Packages.Count > 0)
			{
				var package = _Packages.Dequeue();
				var buffer = Regulus.PhotonExtension.TypeHelper.Serializer(package);
				_Stream.BeginWrite(buffer, 0, buffer.Length, _WriteCompletion, null);
				/*using (var stream = new MemoryStream())
				{
					ProtoBuf.Serializer.Serialize<Package>(stream, package);                    
					var buffer = stream.ToArray();
					_Stream.BeginWrite(buffer, 0, buffer.Length, _WriteCompletion, null);
				} */
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
			_Stream.EndWrite(ar);
			WriteCompletionEvent();
		}

		void Game.IStage.Leave()
		{

		}

		void Game.IStage.Update()
		{

		}
	}
	public class NetworkStreamReadStage : Regulus.Game.IStage
	{
		System.Net.Sockets.NetworkStream _Stream;
		byte[] _Buffer;
		public NetworkStreamReadStage(System.Net.Sockets.NetworkStream stream, int size)
		{
			_Buffer = new byte[size];
			_Stream = stream;
		}
		void Game.IStage.Enter()
		{
			_Stream.BeginRead(_Buffer, 0, _Buffer.Length, _Readed, null);
		}

		public delegate void OnReadCompletion(Package package);
		public event OnReadCompletion ReadCompletionEvent;

		private void _Readed(System.IAsyncResult ar)
		{
			_Stream.EndRead(ar);
			ReadCompletionEvent(Regulus.PhotonExtension.TypeHelper.Deserialize(_Buffer) as Package);

			/*using(var stream = new MemoryStream())
			{
				var package = ProtoBuf.Serializer.Deserialize<Package>(stream);
				ReadCompletionEvent(package);
			}*/
		}

		void Game.IStage.Leave()
		{

		}

		void Game.IStage.Update()
		{

		}
	}
}