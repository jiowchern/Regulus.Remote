using System;
using System.Collections.Generic;


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
                100
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
            Singleton<Log>.Instance.WriteDebug("Init farm data.");

            foreach(var t in _WorkFarmIds)
            {
                var data = _StroageLoad(t);

                data.OnValue += data_on_value => { _FishFarmDatas.Add(data_on_value); };
            }

            Singleton<Log>.Instance.WriteDebug("farm data loading finish");
        }

        private Value<FishFarmData> _StroageLoad(int farm_id)
        {
            var returnValue = new Value<FishFarmData>();

            var val = _FormulaFarmRecorder.Load(farm_id);

            val.OnValue += stage_data =>
            {
                Singleton<Log>.Instance.WriteDebug("_StroageLoad");
                returnValue.SetValue(stage_data);
            };

            return returnValue;
        }
    }
}