using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class SkillFallToInjuryBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnInjury(int damage);
        public event OnInjury DoneEvent;

        Entity _Entity;
        int _Skill;
        public SkillFallToInjuryBehaviorHandler(Entity entity,int skill)
        {
            _Entity = entity;
            _Skill = skill;
        }

        bool Utility.IUpdatable.Update()
        {
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            var commander = _Entity.FindAbility<IBehaviorCommandAbility>();
            

            if (commander != null)
            { 
                commander.CommandEvent += _Injury;
            }
            
        }

        private void _Injury(IBehaviorCommand command)
        {
            var injury = command as BehaviorCommand.Injury;
            if (injury != null)
            {
                var actor = _Entity.FindAbility<IActorPropertyAbility>();
                CastStep result =  actor.QueryCastStep(_Skill);
                if (result != CastStep.Effective)
                {
                    DoneEvent(injury.Value);
                }
            }
            
        }

        void Framework.ILaunched.Shutdown()
        {
            var commander = _Entity.FindAbility<IBehaviorCommandAbility>();


            if (commander != null)
            {
                commander.CommandEvent -= _Injury;
            }
        }
    }
    class IdleToInjuryBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnInjury(int damage);
        public event OnInjury DoneEvent;

        Entity _Entity;
        public IdleToInjuryBehaviorHandler(Entity entity)
        {
            _Entity = entity;
        }

        bool Utility.IUpdatable.Update()
        {
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            var command = _Entity.FindAbility<IBehaviorCommandAbility>();
            if (command != null)
            {
                command.CommandEvent += _Done;
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

        private void _Done(IBehaviorCommand command)
        {
            var cmd = command as BehaviorCommand.Injury;
            if (cmd != null)
            {
                DoneEvent(cmd.Value);
            }
        }

        
    }


  
}
