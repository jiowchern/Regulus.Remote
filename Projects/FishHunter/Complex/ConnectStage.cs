// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the ConnectStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;
using Regulus.Utility;

#endregion

namespace VGame.Project.FishHunter
{
	internal class ConnectStage : IStage
	{
		public event DoneCallback FailEvent;

		public event DoneCallback SuccessEvent;

		private readonly string _Ip;

		private readonly int _Port;

		private readonly INotifier<IConnect> _Provider;

		public ConnectStage(INotifier<IConnect> provider, string ip, int port)
		{
			this._Provider = provider;
			this._Ip = ip;
			this._Port = port;
		}

		void IStage.Enter()
		{
			this._Provider.Supply += this._Provider_Supply;
		}

		void IStage.Leave()
		{
			this._Provider.Supply -= this._Provider_Supply;
		}

		void IStage.Update()
		{
		}

		public delegate void DoneCallback();

		private void _Provider_Supply(IConnect obj)
		{
			obj.Connect(this._Ip, this._Port).OnValue += this._Result;
		}

		private void _Result(bool success)
		{
			if (success)
			{
				this.SuccessEvent();
			}
			else
			{
				this.FailEvent();
			}
		}
	}
}