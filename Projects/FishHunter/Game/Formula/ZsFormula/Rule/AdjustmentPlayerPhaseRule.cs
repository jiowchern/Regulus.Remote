using System.Collections.Generic;
using System.Linq;


using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
    /// <summary>
    ///     玩家阶段起伏的调整
    /// </summary>
    public class AdjustmentPlayerPhaseRule
    {
        private readonly DataVisitor _Visitor;

        private readonly int[] _Randoms;


        public AdjustmentPlayerPhaseRule(DataVisitor visitor)
        {
            _Visitor = visitor;

            _Randoms =
                _Visitor.RandomDatas.Find(x => x.RandomType == DataVisitor.RandomData.RULE.ADJUSTMENT_PLAYER_PHASE)
                        .RandomValue;
        }

        public void Run()
        {
            
            if(_CheckPlayerRecord(_Randoms[0]))
            {
                return;
            }

            // 從VIR00 - VIR03
            CheckFarmBufferType(_Randoms[1])
            ;
        }

        private void CheckFarmBufferType(int random_value)
        {
            var enums =
                EnumHelper.GetEnums<FarmBuffer.BUFFER_TYPE>()
                          .Where(x => x >= FarmBuffer.BUFFER_TYPE.VIR00 && x <= FarmBuffer.BUFFER_TYPE.VIR03);

            foreach(var i in enums)
            {
                var bufferData = _Visitor.Farm.FindBuffer(_Visitor.FocusBufferBlock, i);

                var top = bufferData.Top * bufferData.BufferTempValue.AverageValue;

                if(bufferData.Buffer <= top)
                {
                    continue;
                }

                if(random_value < bufferData.Gate)
                {
                    bufferData.Buffer -= top;

                    _Visitor.PlayerRecord.Status = bufferData.Top * 5;
                    _Visitor.PlayerRecord.BufferValue = top;
                    _Visitor.PlayerRecord.FarmRecords.First(x => x.FarmId == _Visitor.Farm.FarmId).AsnTimes += 1;
                }
                else
                {
                    bufferData.Buffer = 0;
                }
            }
        }

        private bool _CheckPlayerRecord(int random)
        {
            if(_Visitor.PlayerRecord.BufferValue < 0)
            {
                _Visitor.PlayerRecord.Status = 0;
            }

            if(_Visitor.PlayerRecord.Status > 0)
            {
                _Visitor.PlayerRecord.Status--;
            }
            else if(random >= 200)
            {
                // 20%
                return true;
            }

            return false;
        }
    }
}
