using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Data
{
    public class StageLock
    {

        public StageLock()
        {
            Requires = new int[0];
        }
        public int[] Requires { get; set; }

        public int KillCount { get; set; }
        public int Stage { get; set; }
    }
}
