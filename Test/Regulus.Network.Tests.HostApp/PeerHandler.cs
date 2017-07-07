using System;
using Regulus.Framework;
using Regulus.Network.RUDP;
using Regulus.Utility;

namespace Regulus.Network.Tests.HostApp
{
	internal class PeerHandler : IUpdatable<Timestamp>
	{
		private IPeer _Peer;
		readonly int _Id;
		Command _Command;
		private readonly string _CommandString;
		private readonly Utility.Console.IViewer _Viewer;
	  

	    public  PeerHandler(int id ,Command command , Utility.Console.IViewer viwer , IPeer peer)
		{
			_Viewer = viwer;
			_Command = command;
			_Id = id;
			_Peer = peer;
			_CommandString = string.Format("send{0}", _Id);
		}

		void IBootable.Launch()
		{
			_Viewer.WriteLine(string.Format("Accept Transmitter {0} {1}", _Peer.EndPoint ,_Id));
			
	
            _Command.Register<string>(_CommandString, Send);
		}

	   

	    public void Send(string message)
		{
			_Peer.Send(_ToBytes(message));
		}
		public byte[] _ToBytes(string message)
		{
			return System.Text.Encoding.Default.GetBytes(message);
		}

		void IBootable.Shutdown()
		{
			_Viewer.WriteLine(string.Format("Leave Transmitter {0} {1}", _Peer.EndPoint ,_Id));
			
			_Command.Unregister(_CommandString);
		}

		void _Receive(byte[] buffer)
		{
            if(buffer.Length == 0)
                return;
		    ;
			var message = System.Text.Encoding.Default.GetString(buffer);

			_Viewer.WriteLine(String.Format("transmitter{0} : {1}" , _Id , message));
		}

		bool IUpdatable<Timestamp>.Update(Timestamp time)
		{
		    _Receive(_Peer.Receive());
		    return true ;
        }
	}
}