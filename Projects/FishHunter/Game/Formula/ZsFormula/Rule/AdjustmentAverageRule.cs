
using VGame.Project.FishHunter.Common.Data;

using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
	/// <summary>
	///     平均押注的调整
	/// </summary>
	public class AdjustmentAverageRule
	{
		private readonly DataVisitor _DataVisitor;

		private readonly HitRequest _HitRequest;

		public AdjustmentAverageRule(DataVisitor fish_visitor, HitRequest hit_request)
		{
			_DataVisitor = fish_visitor;
			_HitRequest = hit_request;
		}

		public void Run()
		{
			var bufferData = _DataVisitor.Farm.FindDataRoot(
				_DataVisitor.FocusBlockName, 
				FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			// 前1000局，按照实际总玩分/总玩次，获得平均押注
			// 之后，每次减去1/100000，再补上最新的押注
			if(bufferData.TempValueNode.AverageTimes < 1000)
			{
				bufferData.TempValueNode.AverageTimes += 1;

				bufferData.TempValueNode.AverageTotal += _HitRequest.WeaponData.GetTotalBet();

				bufferData.TempValueNode.AverageValue = bufferData.TempValueNode.AverageTotal
														/ bufferData.TempValueNode.AverageTimes;

				if(bufferData.TempValueNode.AverageTimes == 1000)
				{
					bufferData.TempValueNode.AverageTotal = bufferData.TempValueNode.AverageTotal / 1000 * 100000;
				}
			}
			else
			{
				bufferData.TempValueNode.AverageTotal -= bufferData.TempValueNode.AverageTotal / 100000;
				bufferData.TempValueNode.AverageTotal += _HitRequest.WeaponData.GetTotalBet();
				bufferData.TempValueNode.AverageValue = bufferData.TempValueNode.AverageTotal / 100000;
			}
		}
	}
}
