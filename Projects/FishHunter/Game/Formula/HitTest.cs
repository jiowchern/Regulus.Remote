using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VGame.Project.FishHunter.ZsFormula;
using VGame.Project.FishHunter.ZsFormula.DataStructs;

namespace VGame.Project.FishHunter.Formula
{
    public class HitTest : HitBase
    {
        private Regulus.Utility.IRandom _Random;
        private WeaponChancesTable _WeaponChancesTable;
        private ScoreOddsTable _ScoreOddsTable;
        
        public HitTest(Regulus.Utility.IRandom random)
        {
            _InitialWeapon();
            _InitialScore();
            this._Random = random;
        }

        private void _InitialScore()
        {
            var datas = new ScoreOddsTable.Data[] { 
                new ScoreOddsTable.Data { Key = 1, Value = 0.9f } , 
                new ScoreOddsTable.Data { Key = 2, Value = 0.025f },
                new ScoreOddsTable.Data { Key = 3, Value = 0.025f },
                new ScoreOddsTable.Data { Key = 5, Value = 0.025f },
                new ScoreOddsTable.Data { Key = 10, Value = 0.025f }
            };
            _ScoreOddsTable = new ScoreOddsTable(datas);
        }

        private void _InitialWeapon()
        {
            var datas = new WeaponChancesTable.Data[] { 
                new WeaponChancesTable.Data { Key = 0, Value = 0.9f } , 
                new WeaponChancesTable.Data { Key = 2, Value = 0.033f },
                new WeaponChancesTable.Data { Key = 3, Value = 0.033f },
                new WeaponChancesTable.Data { Key = 4, Value = 0.033f }
            };
            _WeaponChancesTable = new WeaponChancesTable(datas);
        }

        public override HitResponse Request(HitRequest request)
        {
            const int MAX_WEPBET = 10000;
            const int MAX_WEPODDS = 10000;
            const short MAX_TOTALHITS = 1000;
            const short MAX_FISHODDS = 1000;
            const long gateOffset = 0x0fffffff;


            if (request.WepBet > MAX_WEPBET)
                return _Miss(request);

            if(request.WepOdds > MAX_WEPODDS)
                return _Miss(request);

            if(request.TotalHits == 0 || request.TotalHits > MAX_TOTALHITS)
                return _Miss(request);

            if (request.FishOdds == 0 || request.FishOdds > MAX_FISHODDS)
                return _Miss(request);

            long gate = 1000;
            gate *= gateOffset;
            gate *= request.WepBet;
            gate /= request.TotalHits;
            gate /= request.FishOdds;
            gate /= 1000;

            if (gate > 0x0fffffff)
                gate = 0x10000000;
            
            var rValue = _Random.NextLong(0,long.MaxValue);
            var value = rValue % 0x10000000;            
            if (value < gate) 
                return _Die(request);

            return _Miss(request);
        }

        private HitResponse _Die(HitRequest request)
        {
            return new HitResponse {
                FishID = request.FishID, 
                DieResult =  FISH_DETERMINATION.DEATH ,
                SpecAsn = (byte)_WeaponChancesTable.Dice(Regulus.Utility.Random.Instance.NextFloat(0, 1)), 
                WepID = request.WepID ,
                WUp = _ScoreOddsTable.Dice(Regulus.Utility.Random.Instance.NextFloat(0, 1))
            };
        }

        private static HitResponse _Miss(HitRequest request)
        {
            return new HitResponse { WepID = request.WepID ,  DieResult = FISH_DETERMINATION.SURVIVAL, FishID = request.FishID, SpecAsn = 0 };
        }
    }
}
