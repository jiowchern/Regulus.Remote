using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{
    class HitRequestConverter
    {
        private IFishStage _Gpi;

        public HitRequestConverter(IFishStage gpi)
        {
            
            this._Gpi = gpi;
        }


        internal void Conver(short wepbet, short totalhits, short fishodds)
        {
            _Gpi.Hit(new HitRequest { WepBet = wepbet , TotalHits = totalhits , FishOdds = fishodds } );
        }
    }
}
