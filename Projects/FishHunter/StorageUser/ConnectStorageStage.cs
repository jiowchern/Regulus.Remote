// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectStorageStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ConnectStorageStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter.Storage
{
	public class ConnectStorageStage : IStage
	{
		public event DoneCallback DoneEvent;

		private readonly string _IpAddress;

		private readonly int _Port;

		private readonly IUser _User;

		public ConnectStorageStage(IUser user, string ipaddress, int port)
		{
			// TODO: Complete member initialization
			this._User = user;
			this._IpAddress = ipaddress;
			this._Port = port;
		}

		void IStage.Enter()
		{
			this._User.Remoting.ConnectProvider.Supply += this._Connect;
		}

		void IStage.Leave()
		{
			this._User.Remoting.ConnectProvider.Supply -= this._Connect;
		}

		void IStage.Update()
		{
		}

		public delegate void DoneCallback(bool result);

		private void _Connect(IConnect obj)
		{
			var result = obj.Connect(this._IpAddress, this._Port);
			result.OnValue += val => { this.DoneEvent(val); };
		}
	}
}