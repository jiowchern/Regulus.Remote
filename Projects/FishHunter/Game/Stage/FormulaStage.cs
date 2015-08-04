// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaStage.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the FormulaStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Regulus.Game;
using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;
using VGame.Project.FishHunter.Formula;

namespace VGame.Project.FishHunter.Stage
{
	internal class FormulaStage : IStage, IFishStageQueryer
	{
		public event DoneCallback OnDoneEvent;

		private readonly ISoulBinder _Binder;

		public FormulaStage(ISoulBinder binder)
		{
			// TODO: Complete member initialization
			this._Binder = binder;
		}

		Value<IFishStage> IFishStageQueryer.Query(long player_id, byte fish_stage)
		{
			switch (fish_stage)
			{
				case 200:
					return null;
				case 100:
					return new FishStage(player_id, fish_stage);
				default:
					return new CsFishStage(player_id, fish_stage);
			}
		}

		void IStage.Enter()
		{
			this._Binder.Bind<IFishStageQueryer>(this);
		}

		void IStage.Leave()
		{
			this._Binder.Unbind<IFishStageQueryer>(this);
		}

		void IStage.Update()
		{
		}
	}
}