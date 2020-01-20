using System;


using Regulus.Utility;

namespace Regulus.Remote
{
	internal class OnlineStage : IStatus
	{
		private event Action _BreakEvent;

		public event Action BreakEvent
		{
			add { _BreakEvent += value; }
			remove { _BreakEvent -= value; }
		}

		private readonly IAgent _Agent;

		private readonly Online _Online;

		private readonly TProvider<IOnline> _OnlineProvider;

		public OnlineStage(IAgent agent, TProvider<IOnline> provider)
		{
			_Agent = agent;
			_OnlineProvider = provider;
			_Online = new Online(agent);
		}

		void IStatus.Enter()
		{
			_Agent.BreakEvent += _BreakEvent;
			_Bind(_OnlineProvider);
		}

		void IStatus.Leave()
		{
			_Unbind(_OnlineProvider);
			_Agent.BreakEvent -= _BreakEvent;
		}

		void IStatus.Update()
		{
			if(_Agent.Connected == false)
			{
				_BreakEvent();
			}
		}

		private void _Bind(IProvider provider)
		{
			provider.Add(_Online);
			provider.Ready(_Online.Id);
		}

		private void _Unbind(IProvider provider)
		{
			provider.Remove(_Online.Id);
		}
	}
}
