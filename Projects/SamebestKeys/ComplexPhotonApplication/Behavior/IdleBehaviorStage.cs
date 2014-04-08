using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.SamebestKeys
{

    class IdleBehaviorStage : IBehaviorStage
    {
        public delegate void OnBattleIdle();
        public event OnBattleIdle BattleIdleEvent;

        public delegate void OnMove();
        public event OnMove MoveEvent;

        public delegate void OnInjury(int damage);
        public event OnInjury InjuryEvent;

        Entity _Entity;

        ITriggerableAbility IBehaviorStage.Ability
        {
            get { return null; }
        }

        void Game.IStage.Enter()
        {
            var ability = _Entity.FindAbility<IMoverAbility>();
            if (ability != null)
                ability.Act(ActionStatue.Idle, 0, 0);
        }

        void Game.IStage.Leave()
        {
            
        }

        void Game.IStage.Update()
        {
            
        }

        void IBehaviorStage.Invoke(IBehaviorCommand command)
        {
            _BattleIdle(command as BehaviorCommand.Idle);
            _Move(command as BehaviorCommand.Move);
            _Injury(command as BehaviorCommand.Injury);
        }

        private void _Injury(BehaviorCommand.Injury injury)
        {
            if (injury != null)
                InjuryEvent(injury.Value);
        }

        private void _Move(BehaviorCommand.Move move)
        {
            if (move != null)
                MoveEvent();    
        }

        private void _BattleIdle(BehaviorCommand.Idle idle)
        {
            if (idle != null )
            {
                BattleIdleEvent();
            }
        }
    }
}
