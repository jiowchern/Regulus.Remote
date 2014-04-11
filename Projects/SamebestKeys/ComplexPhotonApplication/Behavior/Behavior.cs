using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

    interface IBehaviorAbility
    {
        void Update();
    }

    class Behavior : Regulus.Utility.IUpdatable, IBehaviorAbility , IBehaviorCommandAbility
    {
        Regulus.Utility.Updater _SelfUpdater;
        Entity _Entity;
        Regulus.Game.StageMachine _Machine;
        public Behavior(Entity entity)
        {
            _SelfUpdater = new Utility.Updater();
            _SelfUpdater.Add(this);
            _Entity = entity;
            _Machine = new Game.StageMachine();
        }

        bool Utility.IUpdatable.Update()
        {            
            _Machine.Update();
            return true;
        }

        

        void Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
        }

        void Framework.ILaunched.Launch()
        {
            _ToIdle();
        }


        private void _ChangeStage(BehaviorStage.OnBegin begin)
        {
            var stage = new BehaviorStage(begin);            
            _Machine.Push(stage);
        }
        
        private void _ToIdle()
        {
            _ChangeStage(() => 
            {
                var move = new MoveBehaviorHandler(_Entity);                
                var injury = new IdleToInjuryBehaviorHandler(_Entity);
                var skill = new IdleToSkillBehaviorHandler(_Entity);
                
                injury.DoneEvent += _ToInjury;
                move.MoveEvent += _ToMove;
                skill.SkillEvent += _ToSkill;
                
                _EntityAct(ActionStatue.Idle, 0, 0);
                return new IBehaviorHandler[] { injury, move, skill };
            });            
        }

      

        private void _EntityChangeMode()
        {
            var actor = _Entity.FindAbility<IActorPropertyAbility>();
            if (actor != null)
                actor.ChangeMode();
        }

       
        
        private void _ToMove(float direction)
        {
            BehaviorStage.OnBegin begin = () =>
            {
                var move = new MoveBehaviorHandler(_Entity);
                var stop = new StopToIdleBehaviorHandler(_Entity);
                var idle = new MoveToIdleBehaviorHandler(_Entity);
                var injury = new IdleToInjuryBehaviorHandler(_Entity);
                move.MoveEvent += _ToMove;
                stop.DoneEvent += _ToIdle; 
                idle.DoneEvent += _ToIdle;
                injury.DoneEvent += _ToInjury;
                _EntityMove(direction);
                return new IBehaviorHandler[] { move ,stop, idle, injury };
            };

            _ChangeStage(begin);
        }

       
       
        private void _ToInjury(int damage )
        {
            _ChangeStage(() => 
            {
                var idle = new InjuryToIdleBehaviorHandler(1);
                var knockout = new KnockoutBehaviorHandler(_Entity);

                idle.DoneEvent += _ToIdle;
                knockout.DoneEvent += _ToKnockout;
                var ability = _Entity.FindAbility<IActorPropertyAbility>();
                if (ability != null)
                    ability.Injury(damage);
                _EntityAct(ActionStatue.Injury, 0, 0);
                return new IBehaviorHandler[] { knockout, idle };
            });            
        }

        private void _ToKnockout()
        {
            _ChangeStage(() => 
            {
                var idle = new KnockoutToIdleBehaviorHandler(_Entity);
                idle.DoneEvent += _ToIdle;

                var ability = _Entity.FindAbility<IActorPropertyAbility>();
                if (ability != null)
                    ability.DoWakin();
                _EntityAct(ActionStatue.Knockout, 0, 0);
                return new IBehaviorHandler[] { idle };
            });            
        }


        private void _ToSkill(int skill)
        {
            _ChangeStage(() => 
            {
                var ability = _Entity.FindAbility<IActorPropertyAbility>();
                var result = ability.Cast(skill);

                var idle = new SkillHitToIdleBehaviorHandler(_Entity, result);
                var injury = new SkillFallToInjuryBehaviorHandler(_Entity, skill);
                idle.IdleEvent += _ToIdle;
                injury.DoneEvent += _ToInjury;
                _EntityAct(ActionStatue.SkillBegin + skill, 0, 0);

                return new IBehaviorHandler[] { injury, idle };    
            });
            
        }

        

        void _EntityMove(float direction)
        {
            var property = _Entity.FindAbility<IActorPropertyAbility>();
            var move = _Entity.FindAbility<IMoverAbility>();

            if (move != null && property != null)
                move.Act(ActionStatue.Walk, property.CurrentSpeed, direction);
        }

        private void _EntityAct(ActionStatue action_statue, float speed, float direction)
        {
            var move = _Entity.FindAbility<IMoverAbility>();
            if (move != null)
                move.Act(ActionStatue.Idle, 0, 0);
        }

        void IBehaviorAbility.Update()
        {
            _SelfUpdater.Update();
        }

        void IBehaviorCommandAbility.Invoke(IBehaviorCommand command)
        {
            if (_CommandEvent != null)
                _CommandEvent(command);
        }
        event Action<IBehaviorCommand> _CommandEvent;
        event Action<IBehaviorCommand> IBehaviorCommandAbility.CommandEvent
        {
            add { _CommandEvent += value; }
            remove { _CommandEvent -= value; }
        }
    }
}
