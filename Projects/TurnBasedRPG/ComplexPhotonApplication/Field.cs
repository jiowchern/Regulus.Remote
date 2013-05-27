using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Field
    {

        float _Range ;
        public Entity Entity { get; private set; }

        List<Entity> _Within = new List<Entity>();

        public Field(Entity ent)
        {
            Entity = ent;
            _Range = Entity.Vision * Entity.Vision;
        }


        public Action<Entity> IntoEvent;
        public Action<Entity> LeftEvent;
        internal void Update(TurnBasedRPG.Entity[] entitys)
        {
            foreach (var entity in entitys)
            {
                if (_Distance(entity, Entity) > _Range)
                {
                    // out
                    if (_Find(_Within, entity))
                    {
                        //LeftEvent(entity);
                    }
                }
                else
                { 
                    // in
                    if (_Find(_Within, entity) == false)
                    {
                        //_Within.Add(entity);
                        //IntoEvent(entity); 
                    }
                }
            }
        }

        private bool _Find(List<TurnBasedRPG.Entity> within, TurnBasedRPG.Entity entity)
        {
            return within.Remove(entity);
        }

        private float _Distance(TurnBasedRPG.Entity e1, TurnBasedRPG.Entity e2)
        {
            return (e1.Position.X - e2.Position.X) * (e1.Position.X - e2.Position.X) + (e1.Position.Y - e2.Position.Y) * (e1.Position.Y - e2.Position.Y);
        }
    }
}
