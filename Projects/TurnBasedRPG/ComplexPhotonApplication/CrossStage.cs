using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class CrossStage : Regulus.Utility.IStage<User>
    {
        private Regulus.Project.TurnBasedRPG.IWorld _World;
        private string target_map;
        private Regulus.CustomType.Vector2 target_position;
        private string current_map;
        private Regulus.CustomType.Vector2 current_position;

        public CrossStage(Regulus.Project.TurnBasedRPG.IWorld _World, string target_map, Regulus.CustomType.Vector2 target_position, string current_map, Regulus.CustomType.Vector2 current_position)
        {
            // TODO: Complete member initialization
            this._World = _World;
            this.target_map = target_map;
            this.target_position = target_position;
            this.current_map = current_map;
            this.current_position = current_position;
        }

        Utility.StageLock Utility.IStage<User>.Enter(User obj)
        {
            var mapValue = _World.Find(target_map);
            mapValue.OnValue += (map) => 
            {
                if (map == null)
                {
                    obj.ToAdventure(current_map, current_position);
                }
                else
                {
                    obj.ToAdventure(target_map, target_position);
                }

            };

            return null;
        }

        

        void Utility.IStage<User>.Leave(User obj)
        {
           
        }

        void Utility.IStage<User>.Update(User obj)
        {
            
        }
    }
}
