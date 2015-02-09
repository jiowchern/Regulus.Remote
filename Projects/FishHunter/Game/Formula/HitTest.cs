using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{


    public class HitTest : HitBase
    {
        private Regulus.Utility.IRandom _Random;
        
        public HitTest(Regulus.Utility.IRandom random)
        {
            
            this._Random = random;
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
            
            var value = _Random.NextLong(long.MinValue,long.MaxValue) % 0x10000000;
            if(value < gate )
                return _Die(request);

            return _Miss(request);
        }

        private HitResponse _Die(HitRequest request)
        {
            return new HitResponse { FishID = request.FishID, DieResult =  FISH_DETERMINATION.DEATH , SpecAsn = 0, WepID = request.WepID };
        }

        private static HitResponse _Miss(HitRequest request)
        {
            return new HitResponse { WepID = request.WepID ,  DieResult = FISH_DETERMINATION.SURVIVAL, FishID = request.FishID, SpecAsn = 0 };
        }
    }
}
