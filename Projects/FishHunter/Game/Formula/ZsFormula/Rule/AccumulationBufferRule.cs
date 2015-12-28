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
			var enumData = EnumHelper.GetEnums<FarmDataRoot.BufferNode.BUFFER_NAME>();

			foreach(var data 
					in enumData.Select(buffer_type =>
										_Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, buffer_type)))

			{
				_AddBufferRate(data);
			}

			_MoveSpecBufferToNormalBuffer();

			_RecordAll();
		}

		/// <summary>
		///     當block裡的spec type裡有錢大於0就要搬動)，就要把spec的錢移到normal，spec清空
		/// </summary>
		private void _MoveSpecBufferToNormalBuffer()
		{
			var spec = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName,
												FarmDataRoot.BufferNode.BUFFER_NAME.SPEC);

			var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName,
													FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			normal.Buffer.WinScore += spec.Buffer.WinScore;

			spec.Buffer.WinScore = 0;
		}

		private void _AddBufferRate(FarmDataRoot root)
		{
			root.Buffer.Count += _Request.WeaponData.GetTotalBet() * root.Buffer.Rate;

			if(root.Buffer.Count < 1000)
			{
				return;
			}

			root.Buffer.WinScore += root.Buffer.Count / 1000;
			root.Buffer.Count = root.Buffer.Count % 1000;
		}

		/// <summary>
		///     各個block各別加總GetTotalBet，win也要各別計算
		/// </summary>
		private void _RecordAll()
		{
			_RecordToFocusBlock();
			_RecordToFarm();
			_RecordToPlayer();
		}

		private void _RecordToFocusBlock()
		{
			var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName,
												FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			normal.Block.FireCount += 1;
			normal.Block.TotalSpending += _Request.WeaponData.GetTotalBet();
		}

		private void _RecordToPlayer()
		{
			_Visitor.PlayerRecord
					.FindFarmRecord(_Visitor.Farm.FarmId)
					.FireCount += 1;

			_Visitor.PlayerRecord
					.FindFarmRecord(_Visitor.Farm.FarmId)
					.TotalSpending += _Request.WeaponData.GetTotalBet();
		}

		private void _RecordToFarm()
		{
			_Visitor.Farm.Record.FireCount += 1;
			_Visitor.Farm.Record.TotalSpending += _Request.WeaponData.GetTotalBet();
		}
	}
}
