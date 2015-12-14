using NLog;
using NLog.Fluent;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	public class LogHandle
	{
		private readonly DataVisitor _Visitor;

		public LogHandle(DataVisitor visitor)
		{
			_Visitor = visitor;
		}

		public void Run()
		{
			_PlayerLog();
			_FarmDataLog();
		}

		/// <summary>
		///     玩家子彈數每500發，當下區段的buffer value
		/// </summary>
		private void _PlayerLog()
		{
			var playerData = _Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId);

			if(playerData.FireCount % 500 != 0)
			{
				return;
			}

			var log = LogManager.GetLogger("PlayerRecord");
			log.Info()
				.Message("PlayerRecord")
				.Property("FarmId", _Visitor.Farm.FarmId)
				.Property("PlayerId", _Visitor.PlayerRecord.Owner)
				.Property("TotalSpending", playerData.TotalSpending)
				.Property("WinScore", playerData.WinScore)
				.Property("FireCount", playerData.FireCount)
				.Property("AsnTimes", playerData.AsnTimes)
				.Property("AsnWin", playerData.AsnWin)
				.Property("WinFrequency", playerData.WinFrequency)
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
