using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class MoveBehaviorStage : IBehaviorStage
    {
        public delegate void OnIdle();
        public event OnIdle IdleEvent;

        public delegate void OnInjury(int damage);
        public event OnInjury InjuryEvent;

        ITriggerableAbility IBehaviorStage.Ability
        {
            get { return null; }
        }
        Entity _Entity;
        float _Direction;

        public MoveBehaviorStage(Entity entity, float direction)
        {
            _Entity = entity;
            _Direction = direction;
        }
        void Game.IStage.Enter()
        {
            var property = _Entity.FindAbility<IActorPropertyAbility>();
            var move = _Entity.FindAbility<IMoverAbility>();

            if (move != null && property != null)
                move.Act(ActionStatue.Walk, property.NormalSpeed, _Direction);
        }

        void Game.IStage.Leave()
        {            
        }

        void Game.IStage.Update()
        {
            var property = _Entity.FindAbility<IActorPropertyAbility>();
            if (property != null && property.CurrentSpeed == 0.0f)
                IdleEvent();
        }


        void IBehaviorStage.Invoke(IBehaviorCommand command)
        {
            _Injury(command as BehaviorCommand.Injury);
            _Idle(command as BehaviorCommand.Stop);    
        }

        private void _Idle(BehaviorCommand.Stop stop)
        {
            if (stop != null)
                IdleEvent();
        }

        private void _Injury(BehaviorCommand.Injury injury)
        {
            if (injury != null)
                InjuryEvent(injury.Value);
        }

        
    }
}
