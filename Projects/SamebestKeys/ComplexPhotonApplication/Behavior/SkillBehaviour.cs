using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class IdleToSkillBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnSkill(int skill);
        public event OnSkill SkillEvent;


        Entity _Entity;
        public IdleToSkillBehaviorHandler(Entity entity)
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
            var skill = command as Regulus.Project.SamebestKeys.BehaviorCommand.Skill;
            if (skill != null)
            {
                var abiliry = _Entity.FindAbility<IActorPropertyAbility>();
                if (abiliry.HasSkill(skill.Id))
                {
                    SkillEvent(skill.Id);
                }
            }
        }

       
    }
}
