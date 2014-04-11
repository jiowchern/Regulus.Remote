using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    internal interface IBehaviorHandler : Regulus.Utility.IUpdatable
    {
    }

    class CommandBehaviorHandler<T> : IBehaviorHandler where T : class 
    { 
        Entity _Entity;
        public delegate void OnIdle();
        public event OnIdle DoneEvent;
        public delegate bool OnCommand(T command);
        public event OnCommand CommandEvent;

        public CommandBehaviorHandler(Entity entity)
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
                command.CommandEvent += _CommandEvent;
            }
        }

        private void _CommandEvent(IBehaviorCommand obj)
        {
            var command = obj as T;
            if (command != null && CommandEvent(command))
                DoneEvent();
        }

        void Framework.ILaunched.Shutdown()
        {
            var command = _Entity.FindAbility<IBehaviorCommandAbility>();
            if (command != null)
            {
                command.CommandEvent -= _CommandEvent;
            }


        }
    }
}
