using System;
using System.Net.Sockets;

using Regulus.Serialization;
using Regulus.Utiliey;
using Regulus.Network;
using Regulus.Network.Rudp;

using Regulus.Utility;

namespace Regulus.Remote.Ghost
{
	public partial class Agent : IAgent
	{
	    private readonly ISerializer _Serializer;
	    private event Action _BreakEvent;

		private event Action _ConnectEvent;

		private readonly GhostProvider _Core;

		private readonly StatusMachine _Machine;
	    private readonly IConnectProvidable _ConnecterSpawner;
		private readonly TProvider<IConnect> _ConnectProvider;
		private readonly TProvider<IOnline> _OnlineProvider;

		private long _Ping
		{
			get { return _Core.Ping; }
		}

	    public Agent(IProtocol protocol,Regulus.Network.IConnectProvidable connecter_spawner)
	    {	    
            _Serializer = protocol.GetSerialize();
	        _Machine = new StatusMachine();
            _Core = new GhostProvider(protocol);
	        _ConnecterSpawner = connecter_spawner;
			_ConnectProvider = new TProvider<IConnect>();
			_OnlineProvider = new TProvider<IOnline>();
			_Core.AddProvider(typeof(IConnect), _ConnectProvider);
			_Core.AddProvider(typeof(IOnline), _OnlineProvider);
		}

		bool IUpdatable.Update()
		{			
			_Machine.Update();
            return true;
		}

		void IBootable.Launch()
		{
            

            Singleton<Log>.Instance.WriteInfo("Agent Launch.");
            _Core.ErrorMethodEvent += _ErrorMethodEvent;
		    _Core.ErrorVerifyEvent += _ErrorVerifyEvent;

			_ConnecterSpawner.Launch();
			


			_ToConnectStatus();

		}

		void IBootable.Shutdown()
		{
			_ConnecterSpawner.Shutdown();


			_Core.ErrorVerifyEvent -= _ErrorVerifyEvent;
            _Core.ErrorMethodEvent -= _ErrorMethodEvent;
            if (_Core.Enable)
			{
				_ToTermination();

				while(_Core.Enable)
				{
					lock(_Machine)
					{
						_Machine.Update();
					}
				}
			}
			else
			{
				_Machine.Termination();
			}


            Singleton<Log>.Instance.WriteInfo("Agent Shutdown.");
		}

		INotifier<T> INotifierQueryable.QueryNotifier<T>()
		{
			return _Core.QueryProvider<T>();
		}

		/*Value<bool> IAgent.Connect(System.Net.IPEndPoint ip)
		{
			return _Connect(ip);
		}*/

		event Action IAgent.ConnectEvent
		{
			add { _ConnectEvent += value; }
			remove { _ConnectEvent -= value; }
		}

		long IAgent.Ping
		{
			get { return _Ping; }
		}

		event Action IAgent.BreakEvent
		{
			add { _BreakEvent += value; }
			remove { _BreakEvent -= value; }
		}

		/*void IAgent.Disconnect()
		{
			_ToTermination();
		}*/

	    private event Action<string, string> _ErrorMethodEvent;

	    event Action<string, string> IAgent.ErrorMethodEvent
	    {
	        add { this._ErrorMethodEvent += value; }
	        remove { this._ErrorMethodEvent -= value; }
	    }

	    private event Action<byte[], byte[]> _ErrorVerifyEvent;

	    event Action<byte[], byte[]> IAgent.ErrorVerifyEvent
	    {
	        add { this._ErrorVerifyEvent += value; }
	        remove { this._ErrorVerifyEvent -= value; }
	    }

	    bool IAgent.Connected
		{
			get { return _Core.Enable; }
		}

		/*private Value<bool> _Connect(System.Net.IPEndPoint ip)
		{
			return _ToConnect(ip);
		}*/
		
		private void _ToConnectStatus()
		{
			
			var stage = new ConnectStage(_ConnectProvider, _ConnecterSpawner.Spawn());
			stage.DoneEvent += (socket) =>
			{
				_ConnectResult(socket);
			};
			stage.FailEvent += () =>
			{
				_ToConnectStatus();
			};
			_Machine.Push(stage);
		}

		private void _ConnectResult(IPeer peer)
		{
			if (_ConnectEvent != null)
			{
				_ConnectEvent();
			}

			_ToOnlineStatus(peer);
		}

		private void _ToOnlineStatus(IPeer peer)
		{
			var onlineStage = new OnlineStage(peer, _Core  , _Serializer , _OnlineProvider  );
			onlineStage.DoneFromServerEvent += () =>
			{
				if (_BreakEvent != null)
				{
					_BreakEvent();
				}
				_ToConnectStatus();
				
			};

			_Machine.Push(onlineStage);
		}

		private void _ToTermination()
		{
			lock(_Machine)
			{
				_Machine.Push(new TerminationStage(this));
			}
		}

		

        
    }
}
