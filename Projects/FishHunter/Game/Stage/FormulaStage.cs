// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormulaStage.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the FormulaStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;


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

		private StageGeter[] _Stages;

		public FormulaStage(ISoulBinder binder)
		{
			// TODO: Complete member initialization
			this._Binder = binder;

			_Initital(_Stages);
		}

		private void _Initital(StageGeter[] stages)
		{
			//stages[0] = new StageGeter(IStroage , "Zs" , HitBase);// zs
			//stages[1] = new StageGeter(IStroage, "My"  ,myHitBase);// my

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
					return new CsFishStage(player_id, fish_stage  /*,stage member*/);
			}
		}

		void IStage.Enter()
		{
			//create new stage obj; member
			// new Stage(new HitZs , IStorage , )


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

	internal class StageGeter
	{
		public StageGeter(long player_id, byte fish_stage)
		{
			
		}
	}
}