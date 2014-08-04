using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class SpeedChangeBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnMove(float direction, long time);
        public event OnMove MoveEvent;

        private Entity _Entity;
        bool _Run;

        public SpeedChangeBehaviorHandler(Entity entity)
        {            
            this._Entity = entity;
        }

        bool Utility.IUpdatable.Update()
        {
            bool run = _GetRunStatus();
            if (_Run != run)
            {
                MoveEvent(0, LocalTime.Instance.Delta);
            }
            return true;
        }

        private bool _GetRunStatus()
        {
            var ability = _Entity.FindAbility<IActorPropertyAbility>();
            bool run = ability.Energy > 0;
            return run;
        }
        private void _InitialRun()
        {
            _Run = _GetRunStatus();
        }

        void Framework.ILaunched.Launch()
        {
            _InitialRun();
        }

        void Framework.ILaunched.Shutdown()
        {            
        }
    }
}
