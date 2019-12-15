using Regulus.Utility;

namespace Regulus.Remote
{
	internal class OfflineStage : IStage
	{
		public delegate void OnDone();

		public event OnDone DoneEvent;

		private readonly IAgent _Agent;

		private readonly Connect _Connect;

		private readonly TProvider<IConnect> _ConnectProvider;

		public OfflineStage(IAgent agent, TProvider<IConnect> _ConnectProvider)
		{
			_Agent = agent;
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
			if(_Agent.Connected)
			{
				DoneEvent();
			}
		}

		private void _Connect_ConnectedEvent(System.Net.IPEndPoint ip, Value<bool> result)
		{
			var connectResult = _Agent.Connect(ip);
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
