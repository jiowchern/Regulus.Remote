// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the OnlineStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.Utility;

#endregion

namespace Regulus.Remoting
{
	internal class OnlineStage : IStage
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
			this._Agent = agent;
			this._OnlineProvider = provider;
			_Online = new Online(agent);
		}

		void IStage.Enter()
		{
			_Agent.BreakEvent += _BreakEvent;
			_Bind(_OnlineProvider);
		}

		void IStage.Leave()
		{
			_Unbind(_OnlineProvider);
			_Agent.BreakEvent -= _BreakEvent;
		}

		void IStage.Update()
		{
			if (_Agent.Connected == false)
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