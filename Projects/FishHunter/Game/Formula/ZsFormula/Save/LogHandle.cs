using System;
using System.Linq;

using NLog;
using NLog.Fluent;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Save
{
	public class LogHandle : IPipelineElement
	{
		private readonly DataVisitor _Visitor;

		public LogHandle(DataVisitor visitor)
		{
			_Visitor = visitor;
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
			_PlayerLog();
			_FarmDataLog();
		}

		/// <summary>
		///     玩家子彈數每500發，當下區段的buffer value
		/// </summary>
		private void _PlayerLog()
		{
			var farmRecord = _Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId);

			if(farmRecord.FireCount % 500 != 0)
			{
				return;
			}

			var log = LogManager.GetLogger("PlayerRecord");
			log.Info()
				.Message("PlayerRecord")
				.Property("FarmId", _Visitor.Farm.FarmId)
				.Property("PlayerId", _Visitor.PlayerRecord.Owner)
				.Property("TotalSpending", farmRecord.TotalSpending)
				.Property("WinScore", farmRecord.WinScore)
				.Property("FireCount", farmRecord.FireCount)
				.Property("AsnTimes", farmRecord.AsnTimes)
				.Property("AsnWin", farmRecord.AsnWin)
				.Property("WinFrequency", farmRecord.WinFrequency)

					// 超級炮
				.Property("SuperBombCount", farmRecord.RandomTreasures.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.SUPER_BOMB)
													?.Count ?? 0)
				.Property("SuperBombTotalOdds", farmRecord.WeaponHitRecords.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.SUPER_BOMB)
														?.TotalOdds ?? 0)

					// 電網
				.Property("ElectricNetBombCount", farmRecord.RandomTreasures.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.ELECTRIC_NET)
														?.Count ?? 0)
				.Property("ElectricNetBombTotalOdds", farmRecord.WeaponHitRecords.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.ELECTRIC_NET)
															?.TotalOdds ?? 0)

					// 免費炮
				.Property("FreePowerCount", farmRecord.RandomTreasures.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.FREE_POWER)
													?.Count ?? 0)
				.Property("FreePowerTotalOdds", farmRecord.WeaponHitRecords.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.FREE_POWER)
														?.TotalOdds ?? 0)

					// 全畫面
				.Property("ScreenBombCount", farmRecord.RandomTreasures.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.SCREEN_BOMB)
													?.Count ?? 0)
				.Property("ScreenBombTotalOdds", farmRecord.WeaponHitRecords.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.SCREEN_BOMB)
														?.TotalOdds ?? 0)

					// 皮卡丘
				.Property("ThunderBombCount", farmRecord.RandomTreasures.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.THUNDER_BOMB)
													?.Count ?? 0)
				.Property("ThunderBombTotalOdds", farmRecord.WeaponHitRecords.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.THUNDER_BOMB)
														?.TotalOdds ?? 0)

					// 火蛇
				.Property("FireBombCount", farmRecord.RandomTreasures.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.FIRE_BOMB)
													?.Count ?? 0)
				.Property("FireBombTotalOdds", farmRecord.WeaponHitRecords.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.FIRE_BOMB)
														?.TotalOdds ?? 0)

					// 鐵球
				.Property("DamageBombCount", farmRecord.RandomTreasures.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.DAMAGE_BALL)
													?.Count ?? 0)
				.Property("DamageBombTotalOdds", farmRecord.WeaponHitRecords.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.DAMAGE_BALL)
														?.TotalOdds ?? 0)

					// 小章魚
				.Property("OctopusBombCount", farmRecord.RandomTreasures.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.OCTOPUS_BOMB)
													?.Count ?? 0)
				.Property("OctopusBombTotalOdds", farmRecord.WeaponHitRecords.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.OCTOPUS_BOMB)
														?.TotalOdds ?? 0)

					// 大章魚
				.Property("BigOctopusBombCount", farmRecord.RandomTreasures.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.BIG_OCTOPUS_BOMB)
														?.Count ?? 0)
				.Property("BigOctopusBombTotalOdds", farmRecord.WeaponHitRecords.FirstOrDefault(x => x.WeaponType == WEAPON_TYPE.BIG_OCTOPUS_BOMB)
															?.TotalOdds ?? 0)
				.Write();
		}

		/// <summary>
		///     漁場累積的子彈，每500發印一次。
		/// </summary>
		private void _FarmDataLog()
		{
			if(_Visitor.Farm.Record.FireCount % 500 != 0)
			{
				return;
			}

			foreach(var block in EnumHelper.GetEnums<FarmDataRoot.BlockNode.BLOCK_NAME>())
			{
				foreach(var type in EnumHelper.GetEnums<FarmDataRoot.BufferNode.BUFFER_NAME>())
				{
					var farmDataRoot = _Visitor.Farm.FindDataRoot(block, type);
					var tempValueNode = farmDataRoot.TempValueNode;
					var log = LogManager.GetLogger("StageRecord");
					log.Info()
						.Message("StageRecord")
						.Property("FarmId", _Visitor.Farm.FarmId)
						.Property("FarmName", _Visitor.Farm.Name) // 拿掉
						.Property("FarmBaseOdds", _Visitor.Farm.BaseOdds)
						.Property("FarmMaxBet", _Visitor.Farm.MaxBet)
						.Property("FarmRate", _Visitor.Farm.GameRate)
						.Property("FarmBaseOddsCount", _Visitor.Farm.BaseOddsCount)
						.Property("FarmNowBaseOdds", _Visitor.Farm.NowBaseOdds)
						.Property("FarmTotalSpending", _Visitor.Farm.Record.TotalSpending)
						.Property("FarmWinScore", _Visitor.Farm.Record.WinScore)
						.Property("FarmFireCount", _Visitor.Farm.Record.FireCount)
						.Property("BlockName", farmDataRoot.Block.BlockName)
						.Property("BlockTotal", farmDataRoot.Block.TotalSpending)
						.Property("BlockWinScore", farmDataRoot.Block.WinScore)
						.Property("BlockFireCount", farmDataRoot.Block.FireCount)
						.Property("BufferName", farmDataRoot.Buffer.BufferName)
						.Property("BufferWinScore", farmDataRoot.Buffer.WinScore)
						.Property("BufferRate", farmDataRoot.Buffer.Rate)
						.Property("BufferTop", farmDataRoot.Buffer.Top)
						.Property("BufferGate", farmDataRoot.Buffer.Gate)
						.Property("TempFireCount", tempValueNode.FireCount)
						.Property("TempAverageValue", tempValueNode.AverageValue)
						.Property("TempAverageTimes", tempValueNode.AverageTimes)
						.Property("TempAverageTotal", tempValueNode.AverageTotal)
						.Property("TempHiLoRate", tempValueNode.HiLoRate)
						.Property("TempRealTime", tempValueNode.RealTime)
						.Write();
				}
			}
		}
	}
}
