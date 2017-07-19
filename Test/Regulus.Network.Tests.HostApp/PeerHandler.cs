using System;
using System.Net.Sockets;
using Regulus.Framework;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.HostApp
{
	internal class PeerHandler : IUpdatable<Timestamp>
	{
		private ISocket _Peer;
		readonly int _Id;
		Command _Command;
		private readonly string _CommandString;
		private readonly Utility.Console.IViewer _Viewer;
	    private byte[] _Buffer;


	    public  PeerHandler(int id ,Command command , Utility.Console.IViewer viwer , ISocket peer)
		{
			_Viewer = viwer;
			_Command = command;
			_Id = id;
			_Peer = peer;
			_CommandString = string.Format("send{0}", _Id);
		        _Buffer = new byte[Config.PackageSize];
        }

		void IBootable.Launch()
		{
			_Viewer.WriteLine(string.Format("Accept Transmitter {0} {1}", _Peer.RemoteEndPoint ,_Id));
		    
		    _Peer.Receive(_Buffer, 0, _Buffer.Length, _Readed);
            _Command.Register<string>(_CommandString, Send);
		}

	    private void _Readed(int read_count, SocketError error)
	    {
	        var message = System.Text.Encoding.Default.GetString(_Buffer,0,read_count);
	        _Viewer.WriteLine(String.Format("transmitter{0} : {1}", _Id, message));


	        for (int i = 0; i < _Buffer.Length; i++)
	        {
	            _Buffer[i] = 0;
	        }

            _Peer.Receive(_Buffer, 0, _Buffer.Length, _Readed);
        }


	    public void Send(string message)
	    {
	        var buffer = _ToBytes(message);

            _Peer.Send(buffer , 0 , buffer.Length , (send_count , error) => { });
		}
		public byte[] _ToBytes(string message)
		{
			return System.Text.Encoding.Default.GetBytes(message);
		}

		void IBootable.Shutdown()
		{
			_Viewer.WriteLine(string.Format("Leave Transmitter {0} {1}", _Peer.RemoteEndPoint,_Id));
			
			_Command.Unregister(_CommandString);
		}

		

		bool IUpdatable<Timestamp>.Update(Timestamp time)
		{
		    return _Peer.Connected;
		}
	}
}