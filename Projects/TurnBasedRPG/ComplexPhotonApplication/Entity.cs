using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Entity
    {
        
        Regulus.Project.TurnBasedRPG.Serializable.EntityPropertyInfomation _Property;
        public Entity(Regulus.Project.TurnBasedRPG.Serializable.EntityPropertyInfomation property)
        {
            _Property = property;
            Vision = 10;
        }
        public Regulus.Types.Vector2 Position 
        { 
            get 
            {
                return _Property.Position;
            }            
        }
        
        internal void LeftField(Entity entity)
        {
            
        }

        internal void IntoField(Entity entity)
        {
          
        }
        public int Vision { get ; private set; }
    }
}
