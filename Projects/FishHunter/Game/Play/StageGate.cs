using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    internal class StageGate
    {
        Data.StageLock[] _Locks;
        public StageGate(Data.StageLock[] locks)
        {
            _Locks = locks;
        }
        internal IEnumerable<int> FindUnlockStage(IEnumerable<int> passs)
        {
            List<int> unlocks = new List<int>();
            foreach( var l in _Locks)
            {
                var total = l.Requires.Length;
                if(l.Requires.Intersect( passs).Count() == total)
                {
                    unlocks.Add(l.Stage);
                }
            }

            return unlocks.Except(passs);
        }
    }
}
