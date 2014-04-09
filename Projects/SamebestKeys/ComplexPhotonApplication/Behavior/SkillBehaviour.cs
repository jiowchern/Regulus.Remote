using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Behavior
{
    class SkillBehaviorStage : BehaviorStage
    {
        public IdleBehaviorHandler Idle { get; private set; }
        internal override ITriggerableAbility _TriggerableAbility()
        {
            return null;
        }

        internal override IBehaviorHandler[] _Handlers()
        {
            return new IBehaviorHandler[] { Idle };
        }

        protected override void _Begin()
        {
            
        }

        protected override void _End()
        {
            
        }
    }
}
