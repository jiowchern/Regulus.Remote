// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfflineStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the OfflineStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	internal class OfflineStage : IStage
	{
		public event OnDone DoneEvent;

		private readonly IAgent _Agent;

		private readonly Connect _Connect;

		private readonly TProvider<IConnect> _ConnectProvider;

		public OfflineStage(IAgent agent, TProvider<IConnect> _ConnectProvider)
		{
			this._Agent = agent;
			this._ConnectProvider = _ConnectProvider;
			_Connect = new Connect();
		}

		void IStage.Enter()
		{
			_Bind(_ConnectProvider);
		}

		void IStage.Leave()
		{
			_Unbind(_ConnectProvider);
		}

		void IStage.Update()
		{
			if (this._Agent.Connected)
			{
				DoneEvent();
			}
		}

		public delegate void OnDone();

		private void _Connect_ConnectedEvent(string account, int password, Value<bool> result)
		{
			var connectResult = _Agent.Connect(account, password);
			connectResult.OnValue += success => { result.SetValue(success); };
		}

		private void _Bind(IProvider provider)
		{
			_Connect.ConnectedEvent += _Connect_ConnectedEvent;
			provider.Add(_Connect);
			provider.Ready(_Connect.Id);
		}

		private void _Unbind(IProvider provider)
		{
			provider.Remove(_Connect.Id);
			_Connect.ConnectedEvent -= _Connect_ConnectedEvent;
		}
	}
}