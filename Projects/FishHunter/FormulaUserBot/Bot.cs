// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bot.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Bot type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Framework;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPIs;
using VGame.Project.FishHunter.Formula;

#endregion

namespace FormulaUserBot
{
	internal class Bot : IUpdatable
	{
		private static long _IdSn;

		private readonly long _Id;

		private readonly string _IPAddress;

		private readonly StageMachine _Machine;

		private readonly int _Port;

		private readonly IUser _User;

		public Bot(string _IPAddress, int _Port, IUser user)
		{
			// TODO: Complete member initialization
			this._IPAddress = _IPAddress;
			this._Port = _Port;
			this._User = user;
			_Id = ++Bot._IdSn;
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
			var stage = new BotConnectStage(_User, _IPAddress, _Port, _Id);
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