using System;
using System.Collections.Generic;
namespace Regulus.Remoting.Ghost.Native
{

	public class Agent : Regulus.Utility.IUpdatable, Regulus.Remoting.IGhostRequest
	{
		Regulus.Remoting.AgentCore _Core;
		System.Net.Sockets.TcpClient _Tcp;
		Queue<Package> _WaitWiters;
		Regulus.Game.StageMachine _ReadMachine;
		Regulus.Game.StageMachine _WriteMachine;
		string _IpAddress;
		int _Port;
		public Agent(string ipaddress, int port)
		{
			_WaitWiters = new Queue<Package>();
			_IpAddress = ipaddress;
			_Port = port;
			_ReadMachine = new Game.StageMachine();
			_WriteMachine = new Game.StageMachine();
		}

		bool Utility.IUpdatable.Update()
		{
			_ReadMachine.Update();
			_WriteMachine.Update();
			return true;
		}

		void Framework.ILaunched.Launch()
		{
			_Tcp = new System.Net.Sockets.TcpClient();
			_Tcp.BeginConnect(_IpAddress, _Port, _OnConnect, null);
		}

		private void _OnConnect(IAsyncResult ar)
		{
			_Tcp.EndConnect(ar);
			_Core = new Remoting.AgentCore(this);
			_Core.Initial();

			_ToRead();
			_ToWrite();
		}

		private void _ToRead()
		{
			var stage = new NetworkStreamReadStage(_Tcp.GetStream(), _Tcp.ReceiveBufferSize);
			stage.ReadCompletionEvent += (package) =>
			{

				_Core.OnResponse(package.Code, package.Args);
				/*using(var stream = new MemoryStream(buffer))
				{
					var package = ProtoBuf.Serializer.Deserialize<Package>(stream);                    
					_Core.OnResponse(package.Code, package.Args);
				}*/
				_ToRead();
			};
			_ReadMachine.Push(stage);
		}

		private void _ToWrite()
		{
			var stage = new NetworkStreamWriteStage(_Tcp.GetStream(), _WaitWiters);
			stage.WriteCompletionEvent += _ToWrite;
			_WriteMachine.Push(stage);
		}

		void Framework.ILaunched.Shutdown()
		{
			_Tcp.Close();
			if (_Core != null)
				_Core.Finial();
		}

		void Remoting.IGhostRequest.Request(byte code, System.Collections.Generic.Dictionary<byte, byte[]> args)
		{
			_WaitWiters.Enqueue(new Package() { Args = Regulus.Utility.Map<byte, byte[]>.ToMap(args), Code = code });
		}

		public long Ping { get { return _Core.Ping; } }
	}
}