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
                var knockout = new KnockoutBehaviorHandler(_Entity);
                
                injury.DoneEvent += _ToInjury;
                move.MoveEvent += _ToMove;
                skill.SkillEvent += _ToSkill;
                knockout.DoneEvent += _ToKnockout;

                _EntityAct(new Serializable.ActionCommand() { Command = ActionStatue.Idle_1, Turn = true } );
                return new IBehaviorHandler[] { injury, move, skill, knockout };
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

       
       
        private void _ToInjury(BehaviorCommand.Injury injury)
        {
            _ChangeStage(() => 
            {
                var ability = _Entity.FindAbility<IActorPropertyAbility>();

                if (ability != null && ability.Injury(injury.Value, Regulus.Utility.Random.Next(20, 40)))
                {
                    _EntityAct(new Serializable.ActionCommand() { Command = ActionStatue.Injury, Turn = false, Direction = injury.Direction , Speed = 7 , Absolutely = true });
                }
                else
                    _EntityAct(new Serializable.ActionCommand() { Command = ActionStatue.Injury, Turn = true });



                var idle = new InjuryToIdleBehaviorHandler(1);
                var knockout = new KnockoutBehaviorHandler(_Entity);

                idle.DoneEvent += _ToIdle;
                knockout.DoneEvent += _ToKnockout;
                
                
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
                _EntityAct(new Serializable.ActionCommand() { Command = ActionStatue.Knockout, Turn = true } );
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
                _EntityAct(new Serializable.ActionCommand() { Command = ActionStatue.SkillBegin + skill, Turn = true });

                return new IBehaviorHandler[] { injury, idle };    
            });
            
        }

        

        void _EntityMove(float direction)
        {
            var property = _Entity.FindAbility<IActorPropertyAbility>();
            var move = _Entity.FindAbility<IMoverAbility>();

            if (move != null && property != null)
            {
                move.Act(new Serializable.ActionCommand() { Command = ActionStatue.Walk, Direction = direction, Speed = property.CurrentSpeed, Turn = true });                
            }
        }

        private void _EntityAct(Serializable.ActionCommand action_command)
        {
            var move = _Entity.FindAbility<IMoverAbility>();
            if (move != null)
                move.Act(action_command);
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
