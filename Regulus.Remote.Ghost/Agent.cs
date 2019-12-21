using System;
using System.Net.Sockets;

using Regulus.Serialization;
using Regulus.Framework;
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

		private readonly AgentCore _Core;

		private readonly StageMachine _Machine;
	    private readonly IConnectProviderable connectproivder;

	    private long _Ping
		{
			get { return _Core.Ping; }
		}

	    public Agent(IProtocol protocol,Regulus.Network.IConnectProviderable client)
	    {
	    
            _Serializer = protocol.GetSerialize();
	        _Machine = new StageMachine();
            _Core = new AgentCore(protocol);
	        connectproivder = client;
	        
	        
        }

		bool IUpdatable.Update()
		{
			lock(_Machine)
				_Machine.Update();
            return true;
		}

		void IBootable.Launch()
		{
            

            Singleton<Log>.Instance.WriteInfo("Agent Launch.");
            _Core.ErrorMethodEvent += _ErrorMethodEvent;
		    _Core.ErrorVerifyEvent += _ErrorVerifyEvent;

		    

        }

		void IBootable.Shutdown()
		{
		    
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

		INotifier<T> IAgent.QueryNotifier<T>()
		{
			return _Core.QueryProvider<T>();
		}

		Value<bool> IAgent.Connect(System.Net.IPEndPoint ip)
		{
			return _Connect(ip);
		}

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

		void IAgent.Disconnect()
		{
			_ToTermination();
		}

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

		private Value<bool> _Connect(System.Net.IPEndPoint ip)
		{
			return _ToConnect(ip);
		}

		private Value<bool> _ToConnect(System.Net.IPEndPoint ip)
		{
			lock(_Machine)
			{
				var connectValue = new Value<bool>();
				var stage = new ConnectStage(ip, connectproivder);
				stage.ResultEvent += (result, socket) =>
				{
					_ConnectResult(result, socket);
					connectValue.SetValue(result);
				};
				_Machine.Push(stage);
				return connectValue;
			}
		}

		private void _ConnectResult(bool success, IPeer peer)
		{
			if(success)
			{
				if(_ConnectEvent != null)
				{
					_ConnectEvent();
				}

				_ToOnline(_Machine, peer);
			}
			else
			{
				_ToTermination();
			}
		}

		private void _ToOnline(StageMachine machine, IPeer peer)
		{
			var onlineStage = new OnlineStage(peer, _Core , _Serializer);
			onlineStage.DoneFromServerEvent += () =>
			{
				_ToTermination();
				if(_BreakEvent != null)
				{
					_BreakEvent();
				}
			};

			machine.Push(onlineStage);
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
