using System;
using System.Collections.Generic;
using System.Linq;

using NLog;


using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Stage
{
    internal class StageLoadFarmData
    {
        private readonly IFormulaFarmRecorder _FormulaFarmRecorder;

        private readonly List<int> _WorkFarmIds;

        public StageLoadFarmData(IFormulaFarmRecorder formula_farm_recorder)
        {
            _FormulaFarmRecorder = formula_farm_recorder;

            _WorkFarmIds = new List<int>
            {
                100,
				101,
				102,
				103,
				104,
				105,
				106,
				107,
				108,
				109,
				110,
				111
            };
        }

		public Value<FishFarmData> Load(int farm_id)
        {
	        if(!_WorkFarmIds.Contains(farm_id))
	        {
		        throw new Exception("找不到此渔场编号");
	        }

	        return StroageLoad(farm_id);
		}

        private Value<FishFarmData> StroageLoad(int farm_id)
        {
			LogManager.GetCurrentClassLogger().Debug("Init farm data.");

			var returnValue = new Value<FishFarmData>();

            var val = _FormulaFarmRecorder.Load(farm_id);

			val.OnValue += farm_data =>
            {
				LogManager.GetCurrentClassLogger().Debug("Load Farm Data From Stroage.");
                returnValue.SetValue(farm_data);
            };

			LogManager.GetCurrentClassLogger().Debug("Farm data loading finish.");
			return returnValue;
        }
	}
}