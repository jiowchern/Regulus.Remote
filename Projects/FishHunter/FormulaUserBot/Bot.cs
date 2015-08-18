using System;


using Regulus.Framework;
using Regulus.Utility;

using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;

namespace FormulaUserBot
{
	internal class Bot : IUpdatable
	{
		private readonly Guid _Id;

		private readonly string _IpAddress;

		private readonly StageMachine _Machine;

		private readonly int _Port;

		private readonly IUser _User;

		public Bot(string ip_address, int port, IUser user)
		{
			// TODO: Complete member initialization
			_IpAddress = ip_address;
			_Port = port;
			_User = user;
			_Id = Guid.NewGuid();
			_Machine = new StageMachine();
		}

		bool IUpdatable.Update()
		{
			_Machine.Update();
			return true;
		}

		void IBootable.Shutdown()
		{
			_Machine.Termination();
		}

		void IBootable.Launch()
		{
			_ToConnect();
		}

		private void _ToConnect()
		{
			var stage = new BotConnectStage(_User, _IpAddress, _Port, _Id);
			stage.DoneEvent += _ToPlay;
			_Machine.Push(stage);
		}

		private void _ToPlay(IFishStage fish_stage)
		{
			var stage = new BotPlayStage(fish_stage);
			stage.DoneEvent += _ToConnect;
			_Machine.Push(stage);
		}
	}
}
