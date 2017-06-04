using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Regulus.Network.RUDP
{
	public class Host 
	{
		public event System.Action<Peer> AcceptEvent;
		private int _Port;
		private Socket _Socket;
		private byte[] _Buffer;
		private int _Offset;
		private EndPoint _ReceiveEndPoint;

		public Host(int port)
		{
			this._Port = port;

			_Socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
			_Socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, port));

			_ReceiveEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
			_Receive();
		}

		private void _Receive()
		{
			_Socket.BeginReceiveFrom(_Buffer , _Offset , _Buffer.Length , SocketFlags.None ,ref _ReceiveEndPoint , _DoneReceive , null);
		}

		private void _DoneReceive(IAsyncResult ar)
		{
			
			var readsize = _Socket.EndReceiveFrom(ar, ref _ReceiveEndPoint);
			_Receive();
		}
	}
}
