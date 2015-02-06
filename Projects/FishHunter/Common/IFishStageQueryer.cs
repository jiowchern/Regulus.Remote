using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IFishStageQueryer
    {
        Regulus.Remoting.Value<bool> Query(long player_id , byte fish_stage);

    }
}
