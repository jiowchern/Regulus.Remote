using System;
using System.Collections.Generic;
using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	/// <summary>
	///     檢查必掉道具
	/// </summary>
	public class CertainTreasure
	{
		protected DataVisitor _DataVisitor;

		protected RequsetFishData _FishData;

		protected List<WEAPON_TYPE> _GotTreasures;

		public CertainTreasure(DataVisitor data_visitor, RequsetFishData fish_data)
		{
			_DataVisitor = data_visitor;
			_FishData = fish_data;

			_GotTreasures = new List<WEAPON_TYPE>();
		}

		public void Run()
		{
			_GetTreasure();

			var target = _DataVisitor.GetTreasures(TreasureKind.KIND.CERTAIN);

			target.Clear();

			target.AddRange(_GotTreasures);
		}

		private void _GetTreasure()
		{
			if(_FishData.FishType >= FISH_TYPE.TROPICAL_FISH && _FishData.FishType <= FISH_TYPE.SPECIAL_EAT_FISH_CRAZY)
			{
				_GotTreasures.Add(_FishData.FishStatus == FISH_STATUS.KING ? WEAPON_TYPE.KING : WEAPON_TYPE.INVALID);
			}
			else
			{
				var certainWeapons = FishTreasure.Get()
												.Find(x => x.FishType == _FishData.FishType)
												.CertainWeapons;

				foreach(var weapon in certainWeapons)
				{
					_GotTreasures.Add(weapon);
				}
			}
		}
	}
}
