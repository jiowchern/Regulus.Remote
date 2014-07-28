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
            
            public Stop()
            {                
            
            }
        }

        class Move : IBehaviorCommand
        {
            public long Time{ get; private set; }
            public float Direction { get; private set; }
            public Move(float direction , long time)
            {
                Direction = direction;
                Time = time;
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
            
            public Injury(int damage , float direction)
            {
                Direction = direction;
                Value = damage;
            }
            public float Direction { get; private set; }
            public int Value { get; private set; }
        }
    }


    interface IBehaviorCommandAbility
    {
        void Invoke(IBehaviorCommand command);
        event Action<IBehaviorCommand> CommandEvent;
    }

}
