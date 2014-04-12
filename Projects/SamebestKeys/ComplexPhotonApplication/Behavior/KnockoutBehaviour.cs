using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class KnockoutBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnDone();
        public event OnDone DoneEvent;
        Entity _Entity;
        public KnockoutBehaviorHandler(Entity entity)
        {
            _Entity = entity;
        }
        bool Utility.IUpdatable.Update()
        {
            var property = _Entity.FindAbility<IActorPropertyAbility>();
            if (property != null)
            {
                if (property.Died())
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


    
}
