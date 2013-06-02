using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Field
    {
        

        List<IObservedAbility> _Within = new List<IObservedAbility>();

        public event Action<IObservedAbility> IntoEvent;
        public event Action<IObservedAbility> LeftEvent;
        internal void Update(IObserveAbility observe, TurnBasedRPG.IObservedAbility[] entitys)
        {
            var exits = entitys.Except(_Within);
            foreach(var exit in exits)
            {
                _Remove(_Within, exit.Id);
                if (LeftEvent != null)
                    LeftEvent(exit);                
            }

            var range = observe.Vision * observe.Vision;
            foreach (var entity in entitys)
            {
                if (_Distance(entity, observe.Observed) > range)
                {
                    // out
                    if (_Remove(_Within, entity.Id))
                    {
                        if (LeftEvent != null)
                            LeftEvent(entity);
                    }
                }
                else
                { 
                    // in
                    if (_Find(_Within, entity) == false)
                    {
                        _Within.Add(entity);
                        if (IntoEvent != null)
                            IntoEvent(entity);
                    }
                }
            }
        }

        private bool _Find(List<TurnBasedRPG.IObservedAbility> _Within, TurnBasedRPG.IObservedAbility entity)
        {
            return _Within.Find(ent => ent.Id == entity.Id) != null;
        }

        private bool _Remove(List<IObservedAbility> within, Guid id)
        {            
            return within.RemoveAll(ent => ent.Id == id) > 0;
        }

        private float _Distance(IObservedAbility e1, IObservedAbility e2)
        {
            return (e1.Position.X - e2.Position.X) * (e1.Position.X - e2.Position.X) + (e1.Position.Y - e2.Position.Y) * (e1.Position.Y - e2.Position.Y);
        }
        
    }
}
