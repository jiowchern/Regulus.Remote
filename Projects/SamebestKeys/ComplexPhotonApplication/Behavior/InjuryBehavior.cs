using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class InjuryBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnInjury(int damage);
        public event OnInjury DoneEvent;

        bool Utility.IUpdatable.Update()
        {
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            
        }

        void Framework.ILaunched.Shutdown()
        {
            
        }

        void IBehaviorCommandInvoker.Invoke(IBehaviorCommand command)
        {
            var cmd = command as BehaviorCommand.Injury;
            if (cmd != null)
            {
                DoneEvent(cmd.Value);
            }
        }
    }


    class InjuryBehaviorStage : BehaviorStage
    {
        public InjuryToIdleBehaviorHandler Idle { get; private set; }
        public KnockoutBehaviorHandler Knockout { get; private set; }

        Entity _Entity;
        public InjuryBehaviorStage(Entity entity , float interval)
        {
            Idle = new InjuryToIdleBehaviorHandler(interval);
            Knockout = new KnockoutBehaviorHandler(entity);
            _Entity = entity;
        }

        internal override ITriggerableAbility _TriggerableAbility()
        {
            return null;
        }

        internal override IBehaviorHandler[] _Handlers()
        {
            return new IBehaviorHandler[] { Idle, Knockout };
        }

        protected override void _Begin()
        {
            var ability = _Entity.FindAbility<IMoverAbility>();
            if (ability != null)
                ability.Act(ActionStatue.Injury, 0, 0);
        }

        protected override void _End()
        {            
        }
    }
}
