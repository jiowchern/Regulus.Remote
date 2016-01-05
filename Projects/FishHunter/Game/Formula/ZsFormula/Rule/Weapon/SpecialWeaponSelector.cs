using System.Collections.Generic;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Weapon
{
	using System;
	using System.Linq;

	/// <summary>
	///     處理各種武器使用規則
	/// </summary>
	public class SpecialWeaponSelector : IPipelineElement
	{
		internal class Data
		{
			public WEAPON_TYPE WeaponType { get; set; }

			public IFilterable WeaponRule { get; set; }
		}

		private readonly HitRequest _HitRequest;

		private readonly List<Data> _WeaponUseRules;

		public SpecialWeaponSelector(HitRequest request)
		{
			_HitRequest = request;

			_WeaponUseRules = new List<Data>
			{
				new Data
				{
					WeaponType = WEAPON_TYPE.BIG_OCTOPUS_BOMB, 
					WeaponRule = new BigOctopusBomb()
				}, 
				new Data
				{
					WeaponType = WEAPON_TYPE.THUNDER_BOMB, 
					WeaponRule = new ThunderBomb()
				}
			};
		}

		bool IPipelineElement.IsComplete
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IPipelineElement.Connect(IPipelineElement next)
		{
			throw new NotImplementedException();
		}

		void IPipelineElement.Process()
		{
			// Provider.Weaon(WEAPON_TYPE).Process(_HitRequest.FishDatas);
			HandselSelector.Select(_HitRequest.FishDatas);

			_WeaponUseRules.FirstOrDefault(x => x.WeaponType == _HitRequest.WeaponData.WeaponType)
					?.WeaponRule.Filter(_HitRequest.FishDatas);

			// return _HitRequest.FishDatas;
		}
	}
}
