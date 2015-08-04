// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttackChecker.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the AttackChecker type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using VGame.Project.FishHunter.Common.Datas;
using VGame.Project.FishHunter.Formula;
using VGame.Project.FishHunter.ZsFormula.DataStructs;
using VGame.Project.FishHunter.ZsFormula.Rules;

namespace VGame.Project.FishHunter.ZsFormula
{
	public class AttackChecker : HitBase
	{
		private readonly AccumulationBufferRule _AccumulationBufferRule;

		private readonly AdjustmentAverageRule _AdjustmentAverageRule;

		private readonly AdjustmentGameLevelRule _AdjustmentGameLevelRule;

		private readonly AdjustmentPlayerRule _AdjustmentPlayerRule;

		private readonly ApproachBaseOddsRule _ApproachBaseOddsRule;

		private readonly AttackData _AttackData;

		private readonly DeathRule _DeathRule;

		private readonly DiedHandleRule _DiedHandleRule;

		private readonly FishTypeRule _FishTypeRule;

		private readonly SpecialItemRule _SpecialItemRule;

		private readonly SpecialWeaponRule _SpecialWeaponRule;

		private readonly StageDataVisit _StageDataVisit;

		private readonly WeaponPowerRule _WeaponPowerRule;

		private readonly WeaponTypeRule _WeaponTypeRule;

		private Player _Player;

		public AttackChecker(StageDataVisit stage_data_visit, AttackData attack_data)
		{
			_StageDataVisit.GetBufferBlock(attack_data.GetWeaponBet());
			_StageDataVisit = stage_data_visit;

			_AttackData = attack_data;

			_AdjustmentGameLevelRule = new AdjustmentGameLevelRule(_StageDataVisit);
			_ApproachBaseOddsRule = new ApproachBaseOddsRule(_StageDataVisit);
			_AdjustmentAverageRule = new AdjustmentAverageRule(_StageDataVisit);
			_AdjustmentPlayerRule = new AdjustmentPlayerRule(_StageDataVisit, _GetPlayerData());
			_WeaponPowerRule = new WeaponPowerRule(_StageDataVisit, _GetPlayerData());
			_AccumulationBufferRule = new AccumulationBufferRule(_StageDataVisit);
			_DeathRule = new DeathRule(_StageDataVisit);
			_FishTypeRule = new FishTypeRule(_StageDataVisit);
			_WeaponTypeRule = new WeaponTypeRule(_StageDataVisit);
			_DiedHandleRule = new DiedHandleRule(_StageDataVisit);
			_WeaponPowerRule = new WeaponPowerRule(_StageDataVisit, _GetPlayerData());
			_SpecialWeaponRule = new SpecialWeaponRule(_StageDataVisit);

			_SpecialItemRule = new SpecialItemRule(_StageDataVisit);
		}

		public void StartCheck()
		{
			if (_AttackData.WeaponData.WeaponType == WeaponDataTable.Data.WEAPON_TYPE.NORMAL_1
			    || _AttackData.WeaponData.WeaponType == WeaponDataTable.Data.WEAPON_TYPE.FREE_4)
			{
				_CheckIsFirstFire();
			}
			else
			{
				_SpecialWeaponRule.Run(_AttackData);

				_DiedHandleRule.Run(_SpecialWeaponRule.Win, _GetPlayerData());
			}
		}

		/// <summary>
		///     // 只有第一發才能累積buffer
		/// </summary>
		private void _CheckIsFirstFire()
		{
			if (_AttackData.WeaponData.WeaponType != WeaponDataTable.Data.WEAPON_TYPE.NORMAL_1 || _AttackData.AttCount != 1)
			{
				return;
			}

			_AccumulationBufferRule.Run(_AttackData.GetWeaponBet(), _GetPlayerData());

			_ApproachBaseOddsRule.Run();
			_AdjustmentAverageRule.Run(_AttackData.GetWeaponBet());
			_AdjustmentGameLevelRule.Run();
			_AdjustmentPlayerRule.Run();
			_WeaponPowerRule.Run();

			_DeathRule.Run(_AttackData);

			_DiedHandleRule.Run(_DeathRule.Win, _GetPlayerData());

			_SpecialItemRule.Run(_GetPlayerData());

			_FishTypeRule.Run(_AttackData, _GetPlayerData());

			_WeaponTypeRule.Run(_DeathRule.Win, _AttackData, _GetPlayerData());

			Regulus.Utility.Random.Instance.NextInt(0, 0x10000000);
		}

		private Player.Data _GetPlayerData()
		{
			return _Player.FindStageData(_StageDataVisit.NowUseData.StageId);
		}

		public override HitResponse Request(HitRequest request)
		{
			throw new System.NotImplementedException();
		}
	}
}