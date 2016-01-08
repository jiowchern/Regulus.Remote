using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	public class NormalWeaponRateCal
	{
		private readonly HitRequest _Request;

		private readonly DataVisitor _Visitor;

		public NormalWeaponRateCal(DataVisitor visitor, HitRequest request)
		{
			_Visitor = visitor;
			_Request = request;
		}

		public void Cal(RequsetFishData fish_data)
		{
			var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			long dieRate = normal.Buffer.Rate;

			dieRate += normal.TempValueNode.HiLoRate;

			if(_Visitor.PlayerRecord.Status != 0)
			{
				dieRate += 200; // 提高20%
			}

			

			if(dieRate < 0)
			{
				dieRate = 0;
			}

			dieRate *= 0x0FFFFFFF; // 自然死亡率

			dieRate *= _Request.WeaponData.WeaponOdds; // 子弹威力

			dieRate *= fish_data.HitDieRate;

			dieRate /= 1000;

			dieRate /= fish_data.GetRealOdds(); // 翻倍後鱼的倍数

			dieRate /= 1000; // 死亡率换算回实际百分比

			if(dieRate > 0x0FFFFFFF)
			{
				dieRate = 0x10000000; // > 100%
			}

			fish_data.HitDieRate = dieRate;
		}
	}
}
