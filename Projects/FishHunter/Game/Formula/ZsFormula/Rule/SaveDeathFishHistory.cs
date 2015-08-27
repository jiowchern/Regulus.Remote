
using System.Linq;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule
{
    /// <summary>
    ///     記錄特殊魚獲得次數
    /// </summary>
    public class SaveDeathFishHistory
    {
        private readonly DataVisitor _DataVisitor;

        private readonly RequsetFishData _Fish;

        public SaveDeathFishHistory(DataVisitor data_visitor, RequsetFishData fish)
        {
            _Fish = fish;
            _DataVisitor = data_visitor;
        }

        public void Run()
        {
            _SavePlayerHit();

            _SaveFarmHit();
        }

        private void _SaveFarmHit()
        {
            var data = _DataVisitor.Farm.Record.FishHits.First(x => x.FishType == _Fish.FishType)
                       ?? new FishHitRecord
                       {
                           FishType = _Fish.FishType, 
                           KillCount = 0, 
                           WinScore = 0
                       };

            data.KillCount++;
        }

        private void _SavePlayerHit()
        {
            var data = _DataVisitor.PlayerRecord.FindFarmRecord(_DataVisitor.Farm.FarmId)
                                   .FishHits.First(x => x.FishType == _Fish.FishType)
                       ?? new FishHitRecord
                       {
                           FishType = _Fish.FishType, 
                           KillCount = 0, 
                           WinScore = 0
                       };

            data.KillCount++;
        }
    }
}
