using System;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	/// <summary>
	///     平均押注的调整
	/// </summary>
	public class AdjustmentAverage : IPipelineElement
	{
		private readonly DataVisitor _DataVisitor;

		private readonly HitRequest _HitRequest;

		public AdjustmentAverage(DataVisitor fish_visitor, HitRequest hit_request)
		{
			_DataVisitor = fish_visitor;
			_HitRequest = hit_request;
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
			var normal = _DataVisitor.Farm.FindDataRoot(_DataVisitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			// 前1000局，按照实际总玩分/总玩次，获得平均押注
			// 之后，每次减去1/100000，再补上最新的押注
			if(normal.TempValueNode.AverageTimes < 1000)
			{
				normal.TempValueNode.AverageTimes += 1;

				normal.TempValueNode.AverageTotal += _HitRequest.WeaponData.GetTotalBet();

				normal.TempValueNode.AverageValue = normal.TempValueNode.AverageTotal / normal.TempValueNode.AverageTimes;

				if(normal.TempValueNode.AverageTimes == 1000)
				{
					normal.TempValueNode.AverageTotal = normal.TempValueNode.AverageTotal / 1000 * 100000;
				}
			}
			else
			{
				normal.TempValueNode.AverageTotal -= normal.TempValueNode.AverageTotal / 100000;
				normal.TempValueNode.AverageTotal += _HitRequest.WeaponData.GetTotalBet();
				normal.TempValueNode.AverageValue = normal.TempValueNode.AverageTotal / 100000;
			}
		}
	}
}
