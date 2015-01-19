using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    public delegate void CrossAbilityOnMove(string target_map, CustomType.Vector2 target_position);
    public delegate void CrossAbilityOnJump(string target);
    interface ICrossAbility
    {
        void Move(string target_map, CustomType.Vector2 target_position);
        event CrossAbilityOnMove MoveEvent;

        void Jump(string _Target);
        event CrossAbilityOnJump JumpEvent;
    }


    class CrossAbility : ICrossAbility
    {
        
        void ICrossAbility.Move(string target_map, CustomType.Vector2 target_position)
        {            
            if (MoveEvent != null)
                MoveEvent(target_map, target_position);
        }

        public event CrossAbilityOnMove MoveEvent;        

        void ICrossAbility.Jump(string target)
        {
            if (JumpEvent != null)
                JumpEvent(target);
        }

        public event CrossAbilityOnJump JumpEvent;
        
    }
}
