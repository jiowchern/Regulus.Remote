using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    public delegate void CrossAbilityOnMove(string target_map, Types.Vector2 target_position);
    interface ICrossAbility
    {
        void Move(string target_map, Types.Vector2 target_position);
        event CrossAbilityOnMove MoveEvent;
    }


    class CrossAbility : ICrossAbility
    {

        void ICrossAbility.Move(string target_map, Types.Vector2 target_position)
        {
            if (MoveEvent != null)
                MoveEvent(target_map, target_position);
        }

        public event CrossAbilityOnMove MoveEvent;
        
    }
}
