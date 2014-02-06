using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remoting.Soul.Native
{
	partial class TcpController : Application.IController
	{
		System.Net.Sockets.TcpListener _Listener;
		
		float _Timeout;

		System.Collections.Concurrent.ConcurrentQueue<Peer> _NewPeers;
		System.Collections.Generic.List<Peer> _Clients;
		Regulus.Utility.Command _Command;
		Regulus.Utility.Console.IViewer _View;
		Regulus.Utility.FPSCounter _FPS;
		public TcpController(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view)
		{
			_Timeout = 0;
			_NewPeers = new System.Collections.Concurrent.ConcurrentQueue<Peer>();
			_Clients = new List<Peer>();
			_Command = command;
			_View = view;
			_FPS = new Utility.FPSCounter();
		}


		string _Name;
		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}

		public event Game.ConsoleFramework<IUser>.OnSpawnUser UserSpawnEvent;

		public event Game.ConsoleFramework<IUser>.OnSpawnUserFail UserSpawnFailEvent;

		public event Game.ConsoleFramework<IUser>.OnUnspawnUser UserUnpawnEvent;

		public void Look()
		{
			_Command.Register<int, float>("Start", _StartListen);
			_Command.Register("Stop", _StopListen);
			_Command.Register("ConnectCount", () => { _View.WriteLine("Connect Count:" + _Clients.Count.ToString()); });
			_Command.Register("FPS", () => { _View.WriteLine("FPS:" + _FPS.Value.ToString()); });
		}

		public void NotLook()
		{
			_Command.Unregister("Start");
			_Command.Unregister("Stop");
			_Command.Unregister("ConnectCount");
			_Command.Unregister("FPS");
		}
		
		public bool Update()
		{
			
			System.Threading.Thread.Sleep(0);
			_HandlePeer(_Clients, _NewPeers);

			_FPS.Update();
			
			return true;
		}

		public void Launch()
		{
			
		}

		private void _StartListen(int port,float timeout)
		{
			_Timeout = timeout;
			_Listener = System.Net.Sockets.TcpListener.Create(port);
			_Listener.Start();
			_HandleConnect(_Listener, _NewPeers);
		}

		public void Shutdown()
		{
			
		}

		private void _StopListen()
		{
			_Listener.Stop();

			_Timeout = 0;
			_NewPeers = new System.Collections.Concurrent.ConcurrentQueue<Peer>();
			_Clients = new List<Peer>();
		}

		
		
		private void _HandlePeer(List<Peer> clients, System.Collections.Concurrent.ConcurrentQueue<Peer> new_clients)
		{
			_AddPeer(clients, new_clients);

			System.Collections.Concurrent.ConcurrentQueue<Peer> removes = _UpdatePeer(clients);

			_RemovePeer(clients, removes);
		}

		private static void _RemovePeer(List<Peer> clients, System.Collections.Concurrent.ConcurrentQueue<Peer> removes)
		{
			while (removes.Count > 0)
			{
				Peer peer;
				if (removes.TryDequeue(out peer))
				{
					clients.Remove(peer);
				}
			}
		}

		private static System.Collections.Concurrent.ConcurrentQueue<Peer> _UpdatePeer(List<Peer> clients)
		{
			System.Collections.Concurrent.ConcurrentQueue<Peer> removes = new System.Collections.Concurrent.ConcurrentQueue<Peer>();
			Parallel.ForEach<Peer>(clients, c =>
			{
				if (c.Update() == false)
				{
					removes.Enqueue(c);
				}
			});

			return removes;
		}

		private static void _AddPeer(List<Peer> clients, System.Collections.Concurrent.ConcurrentQueue<Peer> new_clients)
		{
			if (new_clients.Count > 0)
			{
				Peer client;
				if (new_clients.TryDequeue(out client))
				{
					
					clients.Add(client);
				}
			}
		}
		private void _HandleConnect(System.Net.Sockets.TcpListener listener, System.Collections.Concurrent.ConcurrentQueue<Peer> clients)
		{
			listener.BeginAcceptTcpClient(_OnAcceptTcpClient, null);
		}

		private void _OnAcceptTcpClient(IAsyncResult ar)
		{
			var client = _Listener.EndAcceptTcpClient(ar);

			_NewPeers.Enqueue(new Peer(client, _Timeout));

			_Listener.BeginAcceptTcpClient(_OnAcceptTcpClient, null);
		}

		
	}

	
}
