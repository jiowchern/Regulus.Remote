// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeaponPowerRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the IRuleCheck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.Linq;

using Regulus.Utility;

using VGame.Project.FishHunter.ZsFormula.DataStructs;

#endregion

namespace VGame.Project.FishHunter.ZsFormula.Rules
{
	public interface IRuleCheck
	{
		void Run();
	}

	public class WeaponPowerRule
	{
		private readonly Player.Data _PlayerData;

		private readonly StageDataVisit _StageDataVisit;

		public WeaponPowerRule(StageDataVisit stage_data_visit, Player.Data player_data)
		{
			_StageDataVisit = stage_data_visit;
			_PlayerData = player_data;
		}

		/// <summary>
		///     是否取得特殊道具（特殊武器）
		/// </summary>
		public void Run()
		{
			if (_PlayerData.NowSpecialWeaponData.IsUsed == true)
			{
				return;
			}

			var enumCount = EnumHelper.GetEnums<WeaponDataTable.Data.WEAPON_POWER>().Count();


			foreach (var weaponower in EnumHelper.GetEnums<WeaponDataTable.Data.WEAPON_POWER>())
			{
				var bufferData = _StageDataVisit.FindBufferData(
					_StageDataVisit.NowUseBlock, 
					StageDataTable.BufferData.BUFFER_TYPE.SPEC);

				var gate = bufferData.Rate / enumCount;
				
				gate = (0x0FFFFFFF / (int)weaponower) * gate;


				gate = gate / 1000;

				if (bufferData.BufferTempValue.HiLoRate >= 0)
				{
					gate *= 2;
				}

				if (bufferData.BufferTempValue.HiLoRate < -200)
				{
					gate /= 2;
				}

				var rand = Random.Instance.NextInt(0, 0x10000000);
				if (rand >= gate)
				{
					continue;
				}

				// 这个值就是，玩家是否可以取到 特殊道具 的值	
				var data = _PlayerData.RecodeData.SpecialWeaponDatas.Find(x => x.SpId == 0);

				//_PlayerData.HaveSpecialWeapon = true;
				//_PlayerData.SpecialWeaponId = (int)weaponower + 2; // 2 3 4 

				var weaponData = _PlayerData.RecodeData.SpecialWeaponDatas.Find(x => x.IsUsed == false);
				weaponData.IsUsed = true;
				_PlayerData.NowSpecialWeaponData = weaponData;

				break;
			}
		}
	}
}