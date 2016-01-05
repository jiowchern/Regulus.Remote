using System.Collections.Generic;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace FormulaUserBot
{
	internal class BotPlayStage : IStage
	{
		public delegate void DoneCallback();

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
			if(_HitTime.Second > HitHandler.Interval)
			{
				var totalHits = Random.Instance.NextInt(1, 5);

				_HitRequest(totalHits);

				_HitTime.Reset();
			}
		}

		private void _HitRequest(int total_hits)
		{
			var hitFishs = _SimulateRequsetFishData(total_hits);

			var weaponData = _SimulateRequestWeaponData(total_hits);

				
			var hitRequest = new HitRequest(hitFishs.ToArray(), weaponData);

			var hitHandler = new HitHandler(_Stage, hitRequest);

			_HitHandlers.Add(hitHandler);
		}

		private List<RequsetFishData> _SimulateRequsetFishData(int total_hits)
		{
			var hitFishs = new List<RequsetFishData>();

			for(var i = 0; i < total_hits; ++i)
			{
				hitFishs.Add(
					new RequsetFishData
					{
						FishId = Random.Instance.NextInt(0, 32767),
						FishOdds = Random.Instance.NextInt(1, 1),
						FishStatus = Random.Instance.NextEnum<FISH_STATUS>(),
						FishType = Random.Instance.NextEnum<FISH_TYPE>()
					});
			}

			return hitFishs;
		}

		private RequestWeaponData _SimulateRequestWeaponData(int total_hits)
		{
			var weapon = new RequestWeaponData
			{
				BulletId = Random.Instance.NextInt(0, 32767),
				WeaponType = Random.Instance.NextEnum<WEAPON_TYPE>(),
				WeaponBet = Random.Instance.NextInt(1, 10000),
				WeaponOdds = 1,
				TotalHits = total_hits,
			};
			return weapon;
		}
	}
}
