// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentConnectStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Agent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Net.Sockets;

using Regulus.Utility;

#endregion

namespace Regulus.Remoting.Ghost.Native
{
	public partial class Agent
	{
		private class ConnectStage : IStage
		{
			public event Action<bool, Socket> ResultEvent;

			private readonly string _Ipaddress;

			private readonly int _Port;

			private readonly Socket _Socket;

			private IAsyncResult _AsyncResult;

			private bool? _Result;

			public ConnectStage(string ipaddress, int port)
			{
				_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				_Socket.NoDelay = true;
				if (ipaddress == null)
				{
					throw new ArgumentNullException();
				}

				this._Ipaddress = ipaddress;
				this._Port = port;
			}

			void IStage.Enter()
			{
				Singleton<Log>.Instance.WriteInfo("connect stage enter.");
				Singleton<Log>.Instance.WriteInfo("Agent connect start .");

				try
				{
					// _Socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
					// _Socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 42255));
					_AsyncResult = _Socket.BeginConnect(_Ipaddress, _Port, _ConnectResult, null);
				}
				catch (Exception e)
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
				if (_Result.HasValue == false && ResultEvent != null)
				{
					var call = ResultEvent;
					ResultEvent = null;
					call(false, null);
				}

				if (_Result.HasValue && _Result.Value == false)
				{
					_Socket.Close();
				}

				Singleton<Log>.Instance.WriteInfo("Agent connect leave.");
			}

			void IStage.Update()
			{
				_InvokeResultEvent();
			}

			private void _ConnectResult(IAsyncResult ar)
			{
				var result = false;
				try
				{
					_Socket.EndConnect(ar);
					result = true;
				}
				catch (SocketException ex)
				{
					Singleton<Log>.Instance.WriteInfo(ex.ToString());
				}
				catch (ObjectDisposedException ode)
				{
					Singleton<Log>.Instance.WriteInfo(ode.ToString());
				}
				finally
				{
					_Result = result;
					Singleton<Log>.Instance.WriteInfo(string.Format("connect result {0}.", _Result));
				}
			}

			private void _InvokeResultEvent()
			{
				if (_Result.HasValue && ResultEvent != null)
				{
					var call = ResultEvent;
					ResultEvent = null;
					call(_Result.Value, _Socket);
				}
			}
		}
	}
}