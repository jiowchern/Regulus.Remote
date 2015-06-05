using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Play
{
    internal class StageTicketInspector
    {
        private StageGate _StageGate;
        HashSet<Data.Stage> _Current;
        int _KillCount;
        public StageTicketInspector(StageGate sg)
        {            
            this._StageGate = sg;
            _Current = new HashSet<Data.Stage>();
        }

        internal void Initial(Data.Stage[] stages)
        {
            _Current = new HashSet<Data.Stage>(stages);
            _Update();
        }
        internal void Kill(int count)
        {
            _KillCount += count;
            _Update();
        }
        internal void Pass(int stage)
        {            
            _Current.Add(new Data.Stage { Id = stage, Pass = true } );
            _Update();
        }

        private void _Update()
        {
            var passs = from stage in _Current where stage.Pass == true select stage.Id;
            foreach( var stage  in _StageGate.FindUnlockStage(passs , _KillCount) )
            {
                _Current.Add(new Data.Stage() { Id = stage, Pass = false } );
            }
        }


        public int[] PlayableStages
        {
            get
            {
                return (from stage in _Current select stage.Id).ToArray();        
            } 
        }
    }
}
