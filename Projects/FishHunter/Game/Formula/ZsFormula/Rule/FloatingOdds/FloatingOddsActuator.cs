using System;
using System.Collections.Generic;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds
{
	/// <summary>
	///     處理浮動倍率
	/// </summary>
	public class FloatingOddsActuator : IPipelineElement
	{
		private readonly HitRequest _HitRequest;

		private readonly List<IFloatingCalculator> _OddsRules;

		public FloatingOddsActuator(HitRequest hit_request)
		{
			_HitRequest = hit_request;

			_OddsRules = new List<IFloatingCalculator>
			{
				new BigOctopus(), 
				new King(),
				new WhaleColor(),
				new WhaleSliver()
            };
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
			foreach(var oddsRule in _OddsRules)
			{
				oddsRule.Calculate(_HitRequest.FishDatas);
			}
		}
	}
}
