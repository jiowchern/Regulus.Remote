using System;

namespace Regulus.Network.RUDP
{
	public class Peer
	{
		public bool Activity { get; }
		public string Address { get; }

		public event Action<byte[]> ReceiveEvent;

		public void Send(byte[] buffer)
		{
			throw new NotImplementedException();
		}
	}
}