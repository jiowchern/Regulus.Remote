using System;
using System.Collections.Generic;


using NLog;


using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Stage
{
    internal class StageLoadFarmData : IStage
    {
        public event Action<List<FishFarmData>> OnDoneEvent;

        private readonly List<FishFarmData> _FishFarmDatas;

        private readonly IFormulaFarmRecorder _FormulaFarmRecorder;

        private readonly List<int> _WorkFarmIds;

        public StageLoadFarmData(IFormulaFarmRecorder formula_farm_recorder)
        {
            _FishFarmDatas = new List<FishFarmData>();

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

        void IStage.Enter()
        {
            _StartLoad();
        }

        void IStage.Leave()
        {
            
        }

        void IStage.Update()
        {
            if(_FishFarmDatas.Count != _WorkFarmIds.Count)
            {
                return;
            }

            OnDoneEvent.Invoke(_FishFarmDatas);
        }

        /// <summary>
        ///     戴入所有營業中的魚場資料
        /// </summary>
        private void _StartLoad()
        {
			LogManager.GetCurrentClassLogger().Debug("Init farm data.");

			foreach (var t in _WorkFarmIds)
            {
                var data = _StroageLoad(t);

				data.OnValue += _Data_OnValue; 
            }

			LogManager.GetCurrentClassLogger().Debug("Farm data loading finish.");
        }

		private void _Data_OnValue(FishFarmData obj)
		{
			_FishFarmDatas.Add(obj); 
		}

		private Value<FishFarmData> _StroageLoad(int farm_id)
        {
            var returnValue = new Value<FishFarmData>();

            var val = _FormulaFarmRecorder.Load(farm_id);

			val.OnValue += farm_data =>
            {
				LogManager.GetCurrentClassLogger().Debug("Load Farm Data From Stroage.");
                returnValue.SetValue(farm_data);
            };

            return returnValue;
        }
	}
}