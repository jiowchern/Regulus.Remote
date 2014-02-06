using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remoting.Soul.Native
{
	partial class TcpController : Application.IController, Regulus.Utility.IUpdatable
	{
		class Peer : Regulus.Remoting.IRequestQueue, Regulus.Remoting.IResponseQueue
		{
			System.Net.Sockets.TcpClient _Client;
			Regulus.Remoting.Soul.SoulProvider _SoulProvider;
			System.Collections.Generic.Queue<Regulus.Remoting.Package> _WaitWrites;
			Regulus.Game.StageMachine _ReadMachine;
			Regulus.Game.StageMachine _WriteMachine;
			float _Timeout;
			Regulus.Utility.TimeCounter _TimeoutCounter;
			public Peer(System.Net.Sockets.TcpClient client, float timeout)
			{
				_Timeout = timeout;
				_TimeoutCounter = new Utility.TimeCounter();
				_Client = client;
				_SoulProvider = new Remoting.Soul.SoulProvider(this, this);
				_WaitWrites = new Queue<Remoting.Package>();

				_ReadMachine = new Game.StageMachine();
				_WriteMachine = new Game.StageMachine();

				_HandleWrite();
				_HandleRead();
			}
			private void _HandleWrite()
			{
				var stage = new NetworkStreamWriteStage(_Client.GetStream(), _WaitWrites);
				stage.WriteCompletionEvent += _HandleWrite;
				_WriteMachine.Push(stage);
			}
			private void _HandleRead()
			{
				var stage = new NetworkStreamReadStage(_Client.GetStream(), _Client.ReceiveBufferSize);
				stage.ReadCompletionEvent += (package) =>
				{
					_HandlePackage(package);

					_HandleRead();
				};
				_ReadMachine.Push(stage);
			}

			private void _HandlePackage(Package package)
			{
				if (package.Code == (byte)ClientToServerPhotonOpCode.Ping)
				{
					_TimeoutCounter.Reset();
					(this as Regulus.Remoting.IResponseQueue).Push((int)ServerToClientPhotonOpCode.Ping, new Dictionary<byte, byte[]>());
				}
			}

			public bool Update()
			{
				if (_Connected())
				{
					_SoulProvider.Update();

					_ReadMachine.Update();
					_WriteMachine.Update();
					return true;
				}
				return false;
			}

			private bool _Connected()
			{
				return _TimeoutCounter.Second < _Timeout;
			}

			void Remoting.IResponseQueue.Push(byte cmd, Dictionary<byte, byte[]> args)
			{
				_WaitWrites.Enqueue(new Regulus.Remoting.Package() { Code = cmd, Args = Regulus.Utility.Map<byte, byte[]>.ToMap(args) });
			}

			event Action<Guid, string, Guid, object[]> Remoting.IRequestQueue.InvokeMethodEvent
			{
				add
				{

				}
				remove
				{

				}
			}

			event Action Remoting.IRequestQueue.BreakEvent
			{
				add { }
				remove { }
			}

			void IRequestQueue.Update()
			{

			}
		}
	}
}
