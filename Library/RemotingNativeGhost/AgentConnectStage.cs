using System;
using System.Net;
using System.Net.Sockets;
using Regulus.Network;
using Regulus.Utility;

namespace Regulus.Remoting.Ghost.Native
{
	public partial class Agent
	{
		private class ConnectStage : IStage
		{
			public event Action<bool, ISocket> ResultEvent;

			private readonly string _Ipaddress;

			private readonly int _Port;

			private readonly ISocketConnectable _Socket;

			private IAsyncResult _AsyncResult;

			private bool? _Result;

			public ConnectStage(string ipaddress, int port , Regulus.Network.RUDP.Agent agent)
			{
                
			    _Socket = new RudpConnecter(agent);


                if (ipaddress == null)
				{
					throw new ArgumentNullException();
				}

				_Ipaddress = ipaddress;
				_Port = port;
			}

			void IStage.Enter()
			{
				Singleton<Log>.Instance.WriteInfo("connect stage enter.");
				Singleton<Log>.Instance.WriteInfo("Agent connect start .");

				try
				{
					// _Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
					// _Socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 42255));
				    
					_Socket.Connect(new IPEndPoint(IPAddress.Parse(_Ipaddress), _Port), _ConnectResult);
				}
				catch(Exception e)
				{
					Singleton<Log>.Instance.WriteInfo(string.Format("begin connect fail {0}.", e));
					ResultEvent(false, null);
				}
				finally
				{
					Singleton<Log>.Instance.WriteInfo("agent connect started .");
				}
			}

			void IStage.Leave()
			{
				if(_Result.HasValue == false && ResultEvent != null)
				{
					var call = ResultEvent;
					ResultEvent = null;
					call(false, null);
				}

				if(_Result.HasValue && _Result.Value == false)
				{
					_Socket.Close();
				}

				Singleton<Log>.Instance.WriteInfo("Agent connect leave.");
			}

			void IStage.Update()
			{
				_InvokeResultEvent();
			}

			private void _ConnectResult(bool result)
			{
			    _Result = result;
			    Singleton<Log>.Instance.WriteInfo(string.Format("connect result {0}.", _Result));
            }

			private void _InvokeResultEvent()
			{
				if(_Result.HasValue && ResultEvent != null)
				{
					var call = ResultEvent;
					ResultEvent = null;
					call(_Result.Value, _Socket);
				}
			}
		}
	}
}
