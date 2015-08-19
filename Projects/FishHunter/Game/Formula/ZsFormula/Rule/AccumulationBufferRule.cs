// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccumulationBufferRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//     算法伺服器-累積buffer規則
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     算法伺服器-累積buffer規則
	/// </summary>
	public class AccumulationBufferRule
	{
		private readonly HitRequest _Request;

		private readonly FarmDataVisitor _FarmVisitor;

		public AccumulationBufferRule(FarmDataVisitor farm_visitor, HitRequest request)
		{
			_FarmVisitor = farm_visitor;
			_Request = request;

		}

		public void Run()
		{
			var bet = _Request.WeaponData.WepOdds * _Request.WeaponData.WepBet;

            var enumData = EnumHelper.GetEnums<FarmBuffer.BUFFER_TYPE>();

		    foreach(var bufferType in enumData)
		    {
                var data = _FarmVisitor.FocusFishFarmData.FindBuffer(_FarmVisitor.FocusBufferBlock, bufferType);
                _AddBufferRate(data, bet);
            }

			_FarmVisitor.FocusFishFarmData.RecordData.PlayTimes += 1;
			_FarmVisitor.FocusFishFarmData.RecordData.PlayTotal += bet;

			_FarmVisitor.PlayerRecord.FindFarmRecord(_FarmVisitor.FocusFishFarmData.FarmId).PlayTimes += 1;
			_FarmVisitor.PlayerRecord.FindFarmRecord(_FarmVisitor.FocusFishFarmData.FarmId).PlayTotal += bet;
		}

		private void _AddBufferRate(FarmBuffer data, int bet)
		{
			data.Count += bet * data.Rate;
			if(data.Count < 1000)
			{
				return;
			}

			data.Buffer += data.Count / 1000;
			data.Count = data.Count % 1000;
		}
	}
}
