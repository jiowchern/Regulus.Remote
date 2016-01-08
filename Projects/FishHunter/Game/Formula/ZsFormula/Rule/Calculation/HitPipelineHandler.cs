using System.Collections.Generic;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.FloatingOdds;
using VGame.Project.FishHunter.Formula.ZsFormula.Rule.Weapon;
using VGame.Project.FishHunter.Formula.ZsFormula.Save;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	public class HitPipelineHandler
	{
		private readonly DataVisitor _DataVisitor;

		private readonly List<IPipelineElement> _PipelineElements;

		private readonly HitRequest _Request;

		public HitPipelineHandler(DataVisitor data_visitor, HitRequest request)
		{
			_DataVisitor = data_visitor;
			_Request = request;
			_PipelineElements = new List<IPipelineElement>();
		}

		public HitPipelineHandler SetFocusBlock()
		{
			var block = CalculationBufferBlock.GetBlock(_Request, _DataVisitor.Farm.MaxBet);

			_DataVisitor.FocusBlockName = block;

			return this;
		}

		public HitPipelineHandler SetFishDieRateFromHitOrder()
		{
			for(var i = 0; i < _Request.FishDatas.Length; ++i)
			{
				_Request.FishDatas[i].HitDieRate = new FishHitAllocateTable().GetAllocateData(_Request.WeaponData.TotalHits, i);
			}

			return this;
		}

		public HitPipelineHandler OddsCalculate()
		{
			_PipelineElements.Add(new Odds(_DataVisitor, _Request));

			return this;
		}

		public HitPipelineHandler LotteryTreasure()
		{
			_PipelineElements.Add(new LotteryTreasureTrigger(_DataVisitor));

			return this;
		}

		public HitPipelineHandler FloatingOddsActuate()
		{
			_PipelineElements.Add(new FloatingOddsActuator(_Request));

			return this;
		}

		public HitPipelineHandler SpecialWeaponSelect()
		{
			_PipelineElements.Add(new SpecialWeaponSelector(_Request));

			return this;
		}

		public HitPipelineHandler AccumulationBuffer()
		{
			if(_Request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL)
			{
				_PipelineElements.Add(new AccumulationBuffer(_DataVisitor, _Request));
			}

			return this;
		}

		public HitPipelineHandler ApproachBaseOdds()
		{
			if(_Request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL)
			{
				_PipelineElements.Add(new ApproachBaseOdds(_DataVisitor));
			}

			return this;
		}

		public HitPipelineHandler AdjustmentAverage()
		{
			if(_Request.WeaponData.WeaponType == WEAPON_TYPE.NORMAL)
			{
				_PipelineElements.Add(new AdjustmentAverage(_DataVisitor, _Request));
			}

			return this;
		}

		public HitPipelineHandler AdjustmentPlayerPhase()
		{
			_PipelineElements.Add(new AdjustmentPlayerPhase(_DataVisitor));

			return this;
		}

		public HitPipelineHandler AdjustmentGameLevel()
		{
			_PipelineElements.Add(new AdjustmentGameLevel(_DataVisitor));

			return this;
		}

		public HitPipelineHandler DieRateCalculate()
		{
			_PipelineElements.Add(new DieRateCalculator(_DataVisitor, _Request));

			return this;
		}

		public HitPipelineHandler Record()
		{
			_PipelineElements.Add(new Record(_DataVisitor, _Request));

			return this;
		}

		public HitPipelineHandler Log()
		{
			_PipelineElements.Add(new LogHandle(_DataVisitor));

			return this;
		}

		public List<HitResponse> Process()
		{
			foreach(var element in _PipelineElements)
			{
				element.Process();
			}

			return new ResponseMaker(_DataVisitor, _Request).MakeAll();
		}
	}
}
