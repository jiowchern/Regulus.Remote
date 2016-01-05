using System;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	public class ApproachBaseOdds : IPipelineElement
	{
		private const int _CheckNumbers = 10000;

		private readonly DataVisitor _Visitor;

		public ApproachBaseOdds(DataVisitor visitor)
		{
			_Visitor = visitor;
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

		/// <summary>
		///     NowBaseOdds 趨近 SetBaseOdds
		/// </summary>
		void IPipelineElement.Process()
		{
			if (_Visitor.Farm.NowBaseOdds > _Visitor.Farm.BaseOdds)
			{
				_ChangeNowBaseOdds(-1);
			}

			if (_Visitor.Farm.NowBaseOdds < _Visitor.Farm.BaseOdds)
			{
				_ChangeNowBaseOdds(1);
			}
		}

		private void _ChangeNowBaseOdds(int value)
		{
			if (_Visitor.Farm.Record.FireCount % _CheckNumbers == 0)
			{
				_Visitor.Farm.NowBaseOdds += value;
			}
		}
	}
}
