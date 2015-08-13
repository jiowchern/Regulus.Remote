using Regulus.Remoting;
using Regulus.Utility;

namespace VGame.Project.FishHunter.Storage
{
	public class ConnectStorageStage : IStage
	{
		public delegate void DoneCallback(bool result);

		public event DoneCallback OnDoneEvent;

		private readonly string _IpAddress;

		private readonly int _Port;

		private readonly IUser _User;

		public ConnectStorageStage(IUser user, string ipaddress, int port)
		{
			// TODO: Complete member initialization
			_User = user;
			_IpAddress = ipaddress;
			_Port = port;
		}

		void IStage.Enter()
		{
			_User.Remoting.ConnectProvider.Supply += _Connect;
		}

		void IStage.Leave()
		{
			_User.Remoting.ConnectProvider.Supply -= _Connect;
		}

		void IStage.Update()
		{
		}

		private void _Connect(IConnect obj)
		{
			var result = obj.Connect(_IpAddress, _Port);
			result.OnValue += val => { OnDoneEvent(val); };
		}
	}
}
