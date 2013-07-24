using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class CrossStage : Regulus.Game.IStage<User>
    {
        private Regulus.Project.TurnBasedRPG.IWorld _World;
        private string target_map;
        private Regulus.Types.Vector2 target_position;
        private string current_map;
        private Regulus.Types.Vector2 current_position;

        public CrossStage(Regulus.Project.TurnBasedRPG.IWorld _World, string target_map, Regulus.Types.Vector2 target_position, string current_map, Regulus.Types.Vector2 current_position)
        {
            // TODO: Complete member initialization
            this._World = _World;
            this.target_map = target_map;
            this.target_position = target_position;
            this.current_map = current_map;
            this.current_position = current_position;
        }

        Game.StageLock Game.IStage<User>.Enter(User obj)
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

        

        void Game.IStage<User>.Leave(User obj)
        {
           
        }

        void Game.IStage<User>.Update(User obj)
        {
            
        }
    }
}
