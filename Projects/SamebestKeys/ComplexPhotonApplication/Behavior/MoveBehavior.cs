using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class MoveBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnMove(float direction);
        public event OnMove MoveEvent;

        public MoveBehaviorHandler()
        {

        }
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
            var move = command as BehaviorCommand.Move;
            if (move != null)
            {
                MoveEvent(move.Direction);
            }
        }
    }

    class MoveBehaviorStage : BehaviorStage
    {
        public IdleBehaviorHandler Idle { get; private set; }

        public InjuryBehaviorHandler Injury { get; private set; }
        
        Entity _Entity;
        float _Direction;

        public MoveBehaviorStage(Entity entity, float direction)
        {
            Idle = new IdleBehaviorHandler(entity );
            Injury = new InjuryBehaviorHandler();
            _Entity = entity;
            _Direction = direction;
        }


        internal override ITriggerableAbility _TriggerableAbility()
        {
            return null;
        }

        internal override IBehaviorHandler[] _Handlers()
        {
            return new IBehaviorHandler[] { Idle, Injury };
        }

        protected override void _Begin()
        {
            var property = _Entity.FindAbility<IActorPropertyAbility>();
            var move = _Entity.FindAbility<IMoverAbility>();

            if (move != null && property != null)
                move.Act(ActionStatue.Walk, property.NormalSpeed, _Direction);
        }

        protected override void _End()
        {
            
        }
    }
}
