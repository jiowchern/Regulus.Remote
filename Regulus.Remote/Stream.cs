using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;


using Regulus.Utility;

namespace Regulus.Remote
{
    
	/*public enum SocketIOResult
	{
		None, 

		Done, 

		Break
	}

	public partial class NetworkStreamWriteStage : IStage
	{
		private class WrittingStage : IStage
		{
			public event Action<SocketIOResult> DoneEvent;

			private readonly byte[] _Buffer;

			private readonly Socket _Socket;

			private IAsyncResult _AsyncResult;

			private SocketIOResult _Result;

			public WrittingStage(Socket socket, byte[] buffer)
			{
				_Socket = socket;
				_Buffer = buffer;
			}

			void IStage.Enter()
			{
				try
				{
					_Result = SocketIOResult.None;
					_AsyncResult = _Socket.BeginSend(_Buffer, 0, _Buffer.Length, 0, _WriteCompletion, null);
				}
				catch(SocketException ex)
				{
					Debug.WriteLine(ex.ToString() + ex.ErrorCode);
					_Result = SocketIOResult.Break;
				}
				catch
				{
					_Result = SocketIOResult.Break;
				}
			}

			void IStage.Leave()
			{
			}

			void IStage.Update()
			{
				if(_Result != SocketIOResult.None)
				{
					DoneEvent(_Result);
				}
			}

			private void _WriteCompletion(IAsyncResult ar)
			{
				try
				{
					_Socket.EndSend(ar);
					_Result = SocketIOResult.Done;
				}
				catch(SocketException ex)
				{
					Debug.WriteLine(ex.ToString() + ex.ErrorCode);
					_Result = SocketIOResult.Break;
				}
				catch
				{
					_Result = SocketIOResult.Break;
				}
			}
		}
	}

	public partial class NetworkStreamWriteStage : IStage
	{
		public event Action ErrorEvent;

		public event Action WriteCompletionEvent;

		private const int _HeadSize = 4;

		private readonly StageMachine _Machine;

		private readonly RequestPackage[] _Packages;

		private readonly Socket _Socket;

		public NetworkStreamWriteStage(Socket socket, RequestPackage[] packages)
		{
			_Socket = socket;
			_Packages = packages;
			_Machine = new StageMachine();
		}

		void IStage.Enter()
		{
			var packages = _Packages;
			var buffer = _CreateBuffer(packages);
			_ToWrite(buffer);
		}

		void IStage.Leave()
		{
		}

		void IStage.Update()
		{
			_Machine.Update();
		}

		private void _ToWrite(byte[] buffer)
		{
			var stage = new WrittingStage(_Socket, buffer);
			stage.DoneEvent += result =>
			{
				if(result == SocketIOResult.Done)
				{
					_Done(buffer.Length);
				}
				else
				{
					ErrorEvent();
				}
			};

			_Machine.Push(stage);
		}

		private byte[] _CreateBuffer(RequestPackage[] packages)
		{
			var buffers = from p in packages select TypeHelper.Serialize(p);

			using(var stream = new MemoryStream())
			{
				foreach(var buffer in buffers)
				{
					stream.Write(BitConverter.GetBytes(buffer.Length), 0, NetworkStreamWriteStage._HeadSize);
					stream.Write(buffer, 0, buffer.Length);
				}

				return stream.ToArray();
			}
		}

		private void _ToHead(byte[] buffer)
		{
			var header = BitConverter.GetBytes(buffer.Length);
			var stage = new WrittingStage(_Socket, header);
			stage.DoneEvent += result =>
			{
				if(result == SocketIOResult.Done)
				{
					_ToBody(buffer);
				}
				else
				{
					ErrorEvent();
				}
			};

			_Machine.Push(stage);
		}

		private void _ToBody(byte[] buffer)
		{
			var stage = new WrittingStage(_Socket, buffer);
			stage.DoneEvent += result =>
			{
				if(result == SocketIOResult.Done)
				{
					_Done(buffer.Length + NetworkStreamWriteStage._HeadSize);
				}
				else
				{
					ErrorEvent();
				}
			};

			_Machine.Push(stage);
		}

		private void _Done(int size)
		{
			WriteCompletionEvent();

			Singleton<NetworkMonitor>.Instance.Write.Set(size);
		}
	}

	public partial class NetworkStreamReadStage : IStage
	{
		private class ReadingStage : IStage
		{
			public event Action<byte[]> DoneEvent;

			private readonly int _Size;

			private readonly Socket _Socket;

			private byte[] _Buffer;

			private int _Offset;

			private SocketIOResult _Result;

			public ReadingStage(Socket socket, int size)
			{
				_Socket = socket;

				_Size = size;
			}

			void IStage.Enter()
			{
				try
				{
					_Buffer = new byte[_Size];
					_Result = SocketIOResult.None;
					_Socket.Receive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);
				}
				catch
				{
					_Result = SocketIOResult.Break;
				}
			}

			void IStage.Leave()
			{
			}

			void IStage.Update()
			{
				if(_Result == SocketIOResult.Done)
				{
					DoneEvent(_Buffer);
				}
				else if(_Result == SocketIOResult.Break)
				{
					DoneEvent(null);
				}
			}

			private void _Readed(IAsyncResult ar)
			{
				try
				{
					var readSize = _Socket.EndReceive(ar);
					if(readSize == 0)
					{
						_Result = SocketIOResult.Break;
						return;
					}

					_Offset += readSize;
					if(_Offset == _Size)
					{
						_Result = SocketIOResult.Done;
					}
					else
					{
						_Result = SocketIOResult.None;
						_Socket.Receive(_Buffer, _Offset, _Buffer.Length - _Offset, 0, _Readed, null);
					}
				}
				catch
				{
					_Result = SocketIOResult.Break;
				}
			}
		}
	}

	public partial class NetworkStreamReadStage : IStage
	{
		public delegate void OnReadCompletion(RequestPackage package);

		public event Action ErrorEvent;

		public event OnReadCompletion ReadCompletionEvent;

		private const int _HeadSize = 4;

		private readonly StageMachine _Machine;

		private readonly Socket _Socket;

		public NetworkStreamReadStage(Socket socket)
		{
			_Socket = socket;
			_Machine = new StageMachine();
		}

		void IStage.Enter()
		{
			_ToHead();
		}

		void IStage.Leave()
		{
		}

		void IStage.Update()
		{
			_Machine.Update();
		}

		private void _ToHead()
		{
			var stage = new ReadingStage(_Socket, NetworkStreamReadStage._HeadSize);
			stage.DoneEvent += buffer =>
			{
				if(buffer != null)
				{
					_ToBody(buffer);
				}
				else
				{
					ErrorEvent();
				}
			};

			_Machine.Push(stage);
		}

		private void _ToBody(byte[] head)
		{
			var bodySize = BitConverter.ToInt32(head, 0);
			var stage = new ReadingStage(_Socket, bodySize);
			stage.DoneEvent += buffer =>
			{
				if(buffer != null)
				{
					_Done(buffer);
				}
				else
				{
					ErrorEvent();
				}
			};

			_Machine.Push(stage);
		}

		private void _Done(byte[] body)
		{
			ReadCompletionEvent(TypeHelper.Deserialize<RequestPackage>(body));
			var size = body.Length + NetworkStreamReadStage._HeadSize;
			Singleton<NetworkMonitor>.Instance.Read.Set(size);
		}
	}*/

	/*public class WaitQueueStage : IStage
	{
		public delegate void DoneCallback(RequestPackage[] packages);

		public event DoneCallback DoneEvent;

		private readonly Regulus.Collection.Queue<RequestPackage> _Packages;

		public WaitQueueStage(Regulus.Collection.Queue<RequestPackage> packages)
		{
			_Packages = packages;
		}

		void IStage.Enter()
		{
		}

		void IStage.Leave()
		{
		}

		void IStage.Update()
		{
			var pkgs = _Packages.DequeueAll();
			if(pkgs.Length > 0)
			{
				DoneEvent(pkgs);
			}
		}
	}*/
}
