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
    
    
    interface IBehaviorStage : Regulus.Game.IStage
    {
        ITriggerableAbility Ability { get;  }
        void Invoke(IBehaviorCommand command);
    }

    

    class InjuryBehaviorStage : IBehaviorStage
    {
        public delegate void OnIdle(bool battle);
        public event OnIdle IdleEvent;

        public delegate void OnKnockout();
        public event OnKnockout KnockoutEvent;

        ITriggerableAbility IBehaviorStage.Ability
        {
            get { throw new NotImplementedException(); }
        }

        
        void Game.IStage.Enter()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Leave()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Update()
        {
            throw new NotImplementedException();
        }


        void IBehaviorStage.Invoke(IBehaviorCommand command)
        {
            throw new NotImplementedException();
        }
    }

    class KnockoutBehaviorStage : IBehaviorStage
    {
        public delegate void OnIdle();
        public event OnIdle IdleEvent;

        ITriggerableAbility IBehaviorStage.Ability
        {
            get { throw new NotImplementedException(); }
        }

        

        void Game.IStage.Enter()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Leave()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Update()
        {
            throw new NotImplementedException();
        }


        void IBehaviorStage.Invoke(IBehaviorCommand command)
        {
            throw new NotImplementedException();
        }
    }

    class BattleIdleBehaviorStage : IBehaviorStage
    {
        public delegate void OnIdle();
        public event OnIdle IdleEvent;

        public delegate void OnSkill();
        public event OnSkill SkillEvent;

        public delegate void OnInjury(int damage);
        public event OnInjury InjuryEvent;

        public delegate void OnMove();
        public event OnMove MoveEvent;

        ITriggerableAbility IBehaviorStage.Ability
        {
            get { throw new NotImplementedException(); }
        }

        

        void Game.IStage.Enter()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Leave()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Update()
        {
            throw new NotImplementedException();
        }


        void IBehaviorStage.Invoke(IBehaviorCommand command)
        {
            throw new NotImplementedException();
        }
    }

    class BattleMoveBehaviorStage : IBehaviorStage
    {
        public delegate void OnInjury(int damage);
        public event OnInjury InjuryEvent;

        public delegate void OnIdle(bool battle);
        public event OnIdle IdleEvent;

        ITriggerableAbility IBehaviorStage.Ability
        {
            get { throw new NotImplementedException(); }
        }

        

        void Game.IStage.Enter()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Leave()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Update()
        {
            throw new NotImplementedException();
        }


        void IBehaviorStage.Invoke(IBehaviorCommand command)
        {
            throw new NotImplementedException();
        }
    }

    class BattleSkillBehaviorStage : IBehaviorStage
    {
        public delegate void OnInjury(int damage);
        public event OnInjury InjuryEvent;

        public delegate void OnIdle();
        public event OnIdle IdleEvent;

        ITriggerableAbility IBehaviorStage.Ability
        {
            get { throw new NotImplementedException(); }
        }

        

        void Game.IStage.Enter()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Leave()
        {
            throw new NotImplementedException();
        }

        void Game.IStage.Update()
        {
            throw new NotImplementedException();
        }


        void IBehaviorStage.Invoke(IBehaviorCommand command)
        {
            throw new NotImplementedException();
        }
    }

    partial class BehavioralAbility : ITriggerableAbility , Regulus.Utility.IUpdatable
    {
        
        Regulus.Game.StageMachine _Machine;
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
            var stage = new IdleBehaviorStage();

            stage.BattleIdleEvent += _ToBattleIdle;
            stage.InjuryEvent += _ToInjury;
            stage.MoveEvent += _ToMove;

            _Machine.Push(stage);
        }

        private void _ToMove()
        {
            var stage = new MoveBehaviorStage(null , 0);

            stage.IdleEvent += _ToIdle;
            stage.InjuryEvent += _ToInjury;

            _Machine.Push(stage);
        }

        private void _ToInjury(int damage )
        {
            var stage = new InjuryBehaviorStage();
            stage.IdleEvent += _ToIdle;
            stage.KnockoutEvent += _ToKnockout;
            _Machine.Push(stage);
        }

        private void _ToKnockout()
        {
            var stage = new KnockoutBehaviorStage();
            stage.IdleEvent += _ToIdle;            
            _Machine.Push(stage);
        }

        private void _ToIdle(bool battle)
        {
            if (battle)
                _ToBattleIdle();
            else
                _ToIdle();
        }

        private void _ToBattleIdle()
        {
            var stage = new BattleIdleBehaviorStage();
            stage.IdleEvent += _ToIdle;
            stage.InjuryEvent += _ToInjury;
            stage.MoveEvent += _ToBattleMove;
            stage.SkillEvent += _ToSkill;

            _Machine.Push(stage);
        }

        private void _ToSkill()
        {
            var stage = new BattleSkillBehaviorStage();
            stage.IdleEvent += _ToBattleIdle;
            stage.InjuryEvent += _ToInjury;            

            _Machine.Push(stage);
        }

        private void _ToBattleMove()
        {
            var stage = new BattleMoveBehaviorStage();
            stage.IdleEvent += _ToIdle;
            stage.InjuryEvent += _ToInjury;            

            _Machine.Push(stage);
        }
    }
}
