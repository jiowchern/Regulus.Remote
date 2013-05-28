using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Field
    {

        
        public Entity Entity { get; private set; }

        List<Entity> _Within = new List<Entity>();

        public Field(Entity ent)
        {
            Entity = ent;
            
        }


        public Action<Entity> IntoEvent;
        public Action<Guid> LeftEvent;
        internal void Update(TurnBasedRPG.Entity[] entitys)
        {
            var range = Entity.Vision * Entity.Vision;
            foreach (var entity in entitys)
            {
                if (_Distance(entity, Entity) > range)
                {
                    // out
                    if (_Remove(_Within, entity.Id))
                    {
                        LeftEvent(entity.Id);
                    }
                }
                else
                { 
                    // in
                    if (_Find(_Within, entity) == false)
                    {
                        _Within.Add(entity);
                        IntoEvent(entity); 
                    }
                }
            }
        }

        private bool _Find(List<TurnBasedRPG.Entity> _Within, TurnBasedRPG.Entity entity)
        {
            return _Within.Find(ent => ent == entity) != null;
        }

        private bool _Remove(List<TurnBasedRPG.Entity> within, Guid id)
        {
            return within.RemoveAll(ent => ent.Id == id) > 0;
        }

        private float _Distance(TurnBasedRPG.Entity e1, TurnBasedRPG.Entity e2)
        {
            return (e1.Position.X - e2.Position.X) * (e1.Position.X - e2.Position.X) + (e1.Position.Y - e2.Position.Y) * (e1.Position.Y - e2.Position.Y);
        }

        internal void Left(List<Guid> left_entitys)
        {
            foreach (var entity in left_entitys)
            {
                if (_Remove(_Within, entity))
                {
                    LeftEvent(entity);
                }
            }
        }
    }
}
