using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class KnockoutBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnDone();
        public event OnDone DoneEvent;
        Entity _Entity;
        public KnockoutBehaviorHandler(Entity entity)
        {
            _Entity = entity;
        }
        bool Utility.IUpdatable.Update()
        {
            var property = _Entity.FindAbility<IActorPropertyAbility>();
            if (property != null)
            {
                if (property.Hp == 0)
                {
                    DoneEvent();
                }
            }
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
            
        }
    }


    class KnockoutBehaviorStage : BehaviorStage
    {
        public KnockoutToIdleBehaviorHandler Idle { get; private set; }


        Entity _Entity;
        public KnockoutBehaviorStage(Entity entity)
        {
            _Entity = entity;
            Idle = new KnockoutToIdleBehaviorHandler(entity);
        }


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
            var ability = _Entity.FindAbility<IMoverAbility>();
            if(ability != null)
            {
                ability.Act(ActionStatue.Knockout, 0, 0);
            }
        }

        protected override void _End()
        {
            
        }
    }
}
