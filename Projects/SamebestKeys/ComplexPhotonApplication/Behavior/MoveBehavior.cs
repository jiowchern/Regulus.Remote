using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class MoveBehaviorHandler : IBehaviorHandler
    {
        public delegate void OnMove(float direction,long time );
        public event OnMove MoveEvent;
        Entity _Entity;
        
        public MoveBehaviorHandler(Entity entity)
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
        

        private void _Done(IBehaviorCommand command)
        {
            var move = command as BehaviorCommand.Move;
            if (move != null)
            {
                MoveEvent(move.Direction, move.Time);
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

    
}
