using System;
using System.Net.Sockets;


using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Remoting.Ghost.Native
{
	public partial class Agent : IAgent
	{
		private event Action _BreakEvent;

		private event Action _ConnectEvent;

		private readonly AgentCore _Core;

		private readonly StageMachine _Machine;

		private long _Ping
		{
			get { return _Core.Ping; }
		}

		private Agent()
		{
			_Machine = new StageMachine();
			_Core = new AgentCore();

		    

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
        }

		void IBootable.Shutdown()
		{
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

		Value<bool> IAgent.Connect(string account, int password)
		{
			return _Connect(account, password);
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

	    bool IAgent.Connected
		{
			get { return _Core.Enable; }
		}

		private Value<bool> _Connect(string ipaddress, int port)
		{
			return _ToConnect(ipaddress, port);
		}

		private Value<bool> _ToConnect(string ipaddress, int port)
		{
			lock(_Machine)
			{
				var connectValue = new Value<bool>();
				var stage = new ConnectStage(ipaddress, port);
				stage.ResultEvent += (result, socket) =>
				{
					_ConnectResult(result, socket);
					connectValue.SetValue(result);
				};
				_Machine.Push(stage);
				return connectValue;
			}
		}

		private void _ConnectResult(bool success, Socket socket)
		{
			if(success)
			{
				if(_ConnectEvent != null)
				{
					_ConnectEvent();
				}

				_ToOnline(_Machine, socket);
			}
			else
			{
				_ToTermination();
			}
		}

		private void _ToOnline(StageMachine machine, Socket socket)
		{
			var onlineStage = new OnlineStage(socket, _Core);
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

		/// <summary>
		///     建立代理器
		/// </summary>
		/// <returns></returns>
		public static IAgent Create()
		{
			return new Agent();
		}
	}
}
