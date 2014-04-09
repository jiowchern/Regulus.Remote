using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    interface IBehaviorCommand
    { 

    }

    namespace BehaviorCommand
    {
        class Idle : IBehaviorCommand
        { 

        }

        class Move : IBehaviorCommand
        {
            public float Direction { get; private set; }
            public Move(float direction)
            {
                Direction = direction;
            }
        }

        class Stop : IBehaviorCommand
        {
        }

        class Injury : IBehaviorCommand
        {

            public Injury(int damage)
            {
                Value = damage;
            }

            public int Value { get; private set; }
        }
    }


    interface IBehaviorCommandInvoker
    {
        void Invoke(IBehaviorCommand command);
    }

    interface IBehaviorStage : Regulus.Game.IStage, IBehaviorCommandInvoker
    {
        ITriggerableAbility Ability { get;  }        
    }


    partial class BehavioralAbility : ITriggerableAbility, Regulus.Utility.IUpdatable, IBehaviorCommandInvoker
    {
        Entity _Entity;
        Regulus.Game.StageMachine _Machine;
        public BehavioralAbility(Entity entity)
        {
            _Entity = entity;
            _Machine = new Game.StageMachine();
        }
        Types.Rect ITriggerableAbility.Bounds
        {
            get {
                if (_CurrentStage != null)
                    return _CurrentStage.Bounds;
                return new Types.Rect(0, 0, 0, 0);
            }
        }

        private ITriggerableAbility _CurrentStage 
        {
            get 
            {
                return (_Machine.Current as IBehaviorStage).Ability;
            }
        }

        void ITriggerableAbility.Interactive(long time, List<Map.EntityInfomation> entitys)
        {
            if (_CurrentStage != null)
            {
                _CurrentStage.Interactive(time, entitys);                
            }

            _Machine.Update();
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

        private void _ToIdle()
        {
            var stage = new IdleBehaviorStage(_Entity, ActionStatue.Idle);

            stage.BattleIdle.DoneEvent += _ToBattleIdle;
            stage.Injury.DoneEvent += _ToInjury;            
            stage.Move.MoveEvent += _ToMove;             

            _Machine.Push(stage);
        }

        private void _ToMove(float direction)
        {
            var stage = new MoveBehaviorStage(_Entity, direction);

            stage.Idle.DoneEvent += _ToIdle;
            stage.Injury.DoneEvent += _ToInjury;

            _Machine.Push(stage);
        }
        private void _ToBattleInjury(int damage)
        {
            var stage = new InjuryBehaviorStage(_Entity, 1);
            stage.Idle.DoneEvent += _ToBattleIdle;
            stage.Knockout.DoneEvent += _ToKnockout;            
            _Machine.Push(stage);
        }
        private void _ToInjury(int damage )
        {
            var stage = new InjuryBehaviorStage(_Entity , 1);            
            stage.Idle.DoneEvent += _ToIdle;
            stage.Knockout.DoneEvent += _ToKnockout;            
            _Machine.Push(stage);
        }

        private void _ToKnockout()
        {
            var stage = new KnockoutBehaviorStage(_Entity);
            stage.Idle.DoneEvent += _ToIdle;
            _Machine.Push(stage);
        }

        
        

        private void _ToBattleIdle()
        {

            var stage = new IdleBehaviorStage(_Entity, ActionStatue.BattleIdle);

            stage.BattleIdle.DoneEvent += _ToIdle;
            stage.Injury.DoneEvent += _ToInjury;
            stage.Move.MoveEvent += _ToBattleMove;            

            _Machine.Push(stage);

            
        }

        private void _ToSkill()
        {
            /*var stage = new BattleSkillBehaviorStage();
            stage.IdleEvent += _ToBattleIdle;
            stage.InjuryEvent += _ToInjury;            

            _Machine.Push(stage);*/
        }

        private void _ToBattleMove(float direction)
        {
            var stage = new MoveBehaviorStage(_Entity, direction);
            stage.Idle.DoneEvent += _ToBattleIdle;
            stage.Injury.DoneEvent += _ToBattleInjury;
            _Machine.Push(stage);
        }

        void IBehaviorCommandInvoker.Invoke(IBehaviorCommand command)
        {
            if (_Machine.Current != null)
                (_Machine.Current as IBehaviorStage).Invoke(command);
                
        }
    }
}
