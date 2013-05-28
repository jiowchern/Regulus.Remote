using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Entity 
    {
        public Field Field { get; private set; }
        Regulus.Project.TurnBasedRPG.Serializable.EntityPropertyInfomation _Property;
        public Entity(Regulus.Project.TurnBasedRPG.Serializable.EntityPropertyInfomation property)
        {
            _Property = property;
            
            Field = new Field(this);            
        }
        public Regulus.Types.Vector2 Position 
        { 
            get 
            {
                return _Property.Position;
            }            
        }
        public int Vision { get { return _Property.Vision; } }
        public Guid Id { get{ return _Property.Id;  } }
    }
}
