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

        
    }
    class StopToIdleBehaviorHandler : IBehaviorHandler
    {
        Entity _Entity;
        public delegate void OnIdle();
        public event OnIdle DoneEvent;
        public StopToIdleBehaviorHandler(Entity entity)
        {
            _Entity = entity;
        }

        bool Utility.IUpdatable.Update()
        {
            return true;   
        }

        void Framework.ILaunched.Launch()
        {
            var ability = _Entity.FindAbility<IObservedAbility>();
            if (ability != null)
            {
                ability.ShowActionEvent += _CheckIdle;
            }
        }

        private void _CheckIdle(Serializable.MoveInfomation obj)
        {
            if (obj.Speed == 0)
            {
                DoneEvent();
            }
        }

        void Framework.ILaunched.Shutdown()
        {
            var ability = _Entity.FindAbility<IObservedAbility>();
            if (ability != null)
            {
                ability.ShowActionEvent -= _CheckIdle;
            }
        }
    }

    
    class MoveToIdleBehaviorHandler : IBehaviorHandler
    {
        Entity _Entity;
        public delegate void OnIdle();
        public event OnIdle DoneEvent;

        public MoveToIdleBehaviorHandler(Entity entity)
        {
            _Entity = entity;
        }

        bool Utility.IUpdatable.Update()
        {
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            

            var command =  _Entity.FindAbility<IBehaviorCommandAbility>();
            if (command != null)
            {
                command.CommandEvent += _Done;
            }
        }

        private void _Done(IBehaviorCommand command)
        {
            var cmd = command as BehaviorCommand.Stop;
            if (cmd != null)
            {
                DoneEvent();
            }
        }

        

        void Framework.ILaunched.Shutdown()
        {
            var command = _Entity.FindAbility<IBehaviorCommandAbility>();
            if (command != null)
            {
                command.CommandEvent -= _Done;
            }

            
        }

        
    }

    class SkillHitToIdleBehaviorHandler : IBehaviorHandler
    {

        public delegate void OnIdle();
        public event OnIdle IdleEvent;
        Entity _Entity;
        
        private Remoting.Value<CastResult> _Result;


        public SkillHitToIdleBehaviorHandler(Entity entity, Remoting.Value<CastResult> result)
        {            
            this._Entity = entity;
            this._Result = result;
        }
        bool Utility.IUpdatable.Update()
        {            
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            
            _Result.OnValue += _CastResult;
        }

        private void _CastResult(CastResult cast_result)
        {
            if (cast_result == CastResult.Hit || cast_result == CastResult.Miss)
                IdleEvent();
        }

        void Framework.ILaunched.Shutdown()
        {
            _Result.OnValue -= _CastResult;
        }

        
    }
}
