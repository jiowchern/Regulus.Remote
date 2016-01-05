using System;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	/// <summary>
	///     // 是否死亡的判断
	/// </summary>
	public class DieRateCalculator : IPipelineElement
	{
		private readonly HitRequest _Request;

		private readonly DataVisitor _Visitor;

		public DieRateCalculator(DataVisitor visitor, HitRequest request)
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
			foreach(var fishData in _Request.FishDatas)
			{
				// 有值代表是特殊武器
				var specialWeaponPower = new SpecialWeaponPowerTable().WeaponPowers.Find(x => x.WeaponType == _Request.WeaponData.WeaponType);

				if(specialWeaponPower != null)
				{
					new SpecialWeaponRateCal(_Visitor, _Request).Cal(fishData);
				}
				else
				{
					new NormalWeaponRateCal(_Visitor, _Request).Cal(fishData);
				}
			}
		}
	}


}