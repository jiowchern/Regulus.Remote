using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    class ScoreOddsTable : Regulus.Game.ChancesTable<int>
    {


        public ScoreOddsTable(Regulus.Game.ChancesTable<int>.Data[] datas)
            : base(datas)
        {
        
        }
    }
}
