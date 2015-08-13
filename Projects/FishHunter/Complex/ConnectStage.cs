using Regulus.Remoting;
using Regulus.Utility;

namespace VGame.Project.FishHunter
{
	internal class ConnectStage : IStage
	{
		public delegate void DoneCallback();

		public event DoneCallback OnFailEvent;

		public event DoneCallback OnSuccessEvent;

		private readonly string _Ip;

		private readonly int _Port;

		private readonly INotifier<IConnect> _Provider;

		public ConnectStage(INotifier<IConnect> provider, string ip, int port)
		{
			_Provider = provider;
			_Ip = ip;
			_Port = port;
		}

		void IStage.Enter()
		{
			_Provider.Supply += _Provider_Supply;
		}

		void IStage.Leave()
		{
			_Provider.Supply -= _Provider_Supply;
		}

		void IStage.Update()
		{
		}

		private void _Provider_Supply(IConnect obj)
		{
			obj.Connect(_Ip, _Port).OnValue += _Result;
		}

		private void _Result(bool success)
		{
			if(success)
			{
				OnSuccessEvent();
			}
			else
			{
				OnFailEvent();
			}
		}
	}
}
