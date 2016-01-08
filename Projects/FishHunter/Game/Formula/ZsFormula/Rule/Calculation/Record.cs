using System;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Save;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	public class Record : IPipelineElement
	{
		private readonly HitRequest _Request;

		private readonly DataVisitor _Visitor;

		public Record(DataVisitor visitor, HitRequest request)
		{
			_Visitor = visitor;
			_Request = request;
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
			_RecordToFocusBlock();
			_RecordToFarm();
			_RecordToPlayer();
		}

		private void _RecordToFocusBlock()
		{
			var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			normal.Block.FireCount += 1;
			normal.Block.TotalSpending += _Request.WeaponData.GetTotalBet();
		}

		private void _RecordToPlayer()
		{
			_Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId)
					.FireCount += 1;

			_Visitor.PlayerRecord.FindFarmRecord(_Visitor.Farm.FarmId)
					.TotalSpending += _Request.WeaponData.GetTotalBet();
		}

		private void _RecordToFarm()
		{
			_Visitor.Farm.Record.FireCount += 1;
			_Visitor.Farm.Record.TotalSpending += _Request.WeaponData.GetTotalBet();
		}
	}
}
