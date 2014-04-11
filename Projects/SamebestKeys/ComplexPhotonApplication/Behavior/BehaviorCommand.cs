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
        class ChangeMode : IBehaviorCommand
        {

        }

        class Stop : IBehaviorCommand
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


        class Skill : IBehaviorCommand
        {            
            public int Id { get; private set; }
            public Skill(int id)
            {
                Id = id;
            }
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


    interface IBehaviorCommandAbility
    {
        void Invoke(IBehaviorCommand command);
        event Action<IBehaviorCommand> CommandEvent;
    }

}
