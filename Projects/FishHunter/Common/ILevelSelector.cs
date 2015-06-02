using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface ILevelSelector
    {
        Regulus.Remoting.Value<int[]> QueryStages();

        Regulus.Remoting.Value<bool> Select(int level);
    }
}
