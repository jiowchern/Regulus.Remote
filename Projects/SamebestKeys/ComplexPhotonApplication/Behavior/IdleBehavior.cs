using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.SamebestKeys
{

    class KnockoutToIdleBehaviorHandler : IBehaviorHandler
    {

        public delegate void OnDone();
        public event OnDone DoneEvent;
        Entity _Entity;
        public KnockoutToIdleBehaviorHandler(Entity entiry)
        {
            _Entity = entiry;
        }
        bool Utility.IUpdatable.Update()
        {
            var ability = _Entity.FindAbility<IActorPropertyAbility>();
            if (ability != null)
            {
                if (ability.IsAlive())
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

    class InjuryToIdleBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnDone();
        public event OnDone DoneEvent;
        Regulus.Utility.Timer _Timer;
        public InjuryToIdleBehaviorHandler(float interval)
        {            
            _Timer = new Utility.Timer(interval, _Timeup);
        }

        private void _Timeup(long obj)
        {
            DoneEvent();
        }

        bool Utility.IUpdatable.Update()
        {
            _Timer.Update( LocalTime.Instance.Delta );
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

    class IdleBehaviorHandler : IBehaviorHandler
    {
        Entity _Entity;
        public delegate void OnIdle();
        public event OnIdle DoneEvent;

        public IdleBehaviorHandler(Entity entity)
        {
            _Entity = entity;
        }

        bool Utility.IUpdatable.Update()
        {
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            var observed = _Entity.FindAbility<IObservedAbility>();
            if (observed != null)
            {
                observed.ShowActionEvent += _CheckIdle;
            }
        }

        void _CheckIdle(Serializable.MoveInfomation obj)
        {
            if (obj.Speed == 0)
            {
                DoneEvent();
            }
        }

        void Framework.ILaunched.Shutdown()
        {
            var observed = _Entity.FindAbility<IObservedAbility>();
            if (observed != null)
            {
                observed.ShowActionEvent -= _CheckIdle;
            }
        }

        void IBehaviorCommandInvoker.Invoke(IBehaviorCommand command)
        {
            var cmd = command as BehaviorCommand.Idle;
            if (cmd != null)
            {
                DoneEvent();
            }
        }
    }

    class IdleBehaviorStage : BehaviorStage 
    {
        public IdleBehaviorHandler BattleIdle { get; private set; }

        public MoveBehaviorHandler Move { get; private set; }
        public InjuryBehaviorHandler Injury { get; private set; }

        Entity _Entity;
        ActionStatue _Idle;
        public IdleBehaviorStage(Entity entity , ActionStatue idle)            
        {
            Move = new MoveBehaviorHandler();
            BattleIdle = new IdleBehaviorHandler(entity);
            Injury = new InjuryBehaviorHandler();
            _Entity = entity;
            _Idle = idle;
        }

        internal override IBehaviorHandler[] _Handlers()
        {
            return new IBehaviorHandler[] { Move, BattleIdle , Injury};
        }

        internal override ITriggerableAbility _TriggerableAbility()
        {
            return null;
        }

        protected override void _Begin()
        {
            var ability = _Entity.FindAbility<IMoverAbility>();
            if (ability != null)
                ability.Act(_Idle, 0, 0);
        }

        protected override void _End()
        {
            
        }
    }
}
