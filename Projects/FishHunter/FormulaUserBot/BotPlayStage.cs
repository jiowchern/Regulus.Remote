// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BotPlayStage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the BotPlayStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;


using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace FormulaUserBot
{
	internal class BotPlayStage
		: IStage
	{
		public event DoneCallback DoneEvent;

		private readonly Updater _HitHandlers;

		private readonly TimeCounter _HitTime;

		private readonly IFishStage _Stage;

		public BotPlayStage(IFishStage fish_stage)
		{
			_HitHandlers = new Updater();
			_HitTime = new TimeCounter();
			_Stage = fish_stage;
		}

		void IStage.Leave()
		{
			_HitHandlers.Shutdown();
		}

		void IStage.Enter()
		{
		}

		void IStage.Update()
		{
			_HitHandlers.Working();
			if (_HitTime.Second > HitHandler.Interval)
			{
				// var totalHits = (byte)Regulus.Utility.Random.Next(1, 1000);
				var totalHits = 1;

				for (var i = 0; i < totalHits; ++i)
				{
					_HitRequest(totalHits);
				}

				_HitTime.Reset();
			}
		}

		public delegate void DoneCallback();

		private void _HitRequest(int total_hits)
		{

			var fishDatas = new List<RequsetFishData>
			{
				new RequsetFishData
				{
					FishID = Random.Instance.NextInt(0, 32767),
					FishOdds = Random.Instance.NextInt(1, 1000),
					FishStatus = Random.Instance.NextEnum<FISH_STATUS>(),
					FishType = Random.Instance.NextEnum<FISH_TYPE>()
				}
			};


			var weapon = new RequestWeaponData
			{
				WepID = Random.Instance.NextInt(0, 32767),

				WeaponType = Random.Instance.NextEnum<WEAPON_TYPE>(),

				WepBet = Random.Instance.NextInt(1, 10000),

				WepOdds = Random.Instance.NextInt(1, 10000),

				TotalHits = total_hits,

				TotalHitOdds = Random.Instance.NextInt(0, 32767)
			};
			
			var hitRequest = new HitRequest(fishDatas.ToArray(), weapon);

			var hitHandler = new HitHandler(_Stage, hitRequest);
			_HitHandlers.Add(hitHandler);
		}
	}
}