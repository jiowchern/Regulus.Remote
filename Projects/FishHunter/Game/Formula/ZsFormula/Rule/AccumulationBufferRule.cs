// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccumulationBufferRule.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//     算法伺服器-累積buffer規則
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;


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

		private readonly DataVisitor _Visitor;

		public AccumulationBufferRule(DataVisitor visitor, HitRequest request)
		{
			_Visitor = visitor;
			_Request = request;

		}

		public void Run()
		{
			var bet = _Request.WeaponData.WepOdds * _Request.WeaponData.WepBet;

            var enumData = EnumHelper.GetEnums<FarmBuffer.BUFFER_TYPE>();

		    foreach(var data in enumData.Select(buffer_type => _Visitor.Farm.FindBuffer(_Visitor.FocusBufferBlock, buffer_type)))
		    {
		        _AddBufferRate(data, bet);
		    }

			_Visitor.Farm.Record.PlayTimes += 1;
			_Visitor.Farm.Record.PlayTotal += bet;

			_Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId).PlayTimes += 1;
			_Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId).PlayTotal += bet;
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
