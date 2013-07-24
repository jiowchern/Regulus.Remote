using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Field
    {


        List<PhysicalAbility> _Within = new List<PhysicalAbility>();

        public event Action<IObservedAbility> IntoEvent;
        public event Action<IObservedAbility> LeftEvent;
        internal void Update(IObserveAbility observe, TurnBasedRPG.PhysicalAbility[] entitys, List<IObservedAbility> lefts)
        {
            
            foreach (var exit in lefts)
            {
                    _Remove(_Within, exit.Id);
                    if (LeftEvent != null)
                        LeftEvent(exit);                
                
            }

            foreach(var e in entitys.Except(_Within))
            {
                if (e.ObservedAbility != null)
                {
                    if (IntoEvent != null)
                        IntoEvent(e.ObservedAbility);
                }
                
            }
            foreach (var e in _Within.Except(entitys))
            {
                if (e.ObservedAbility != null)
                {
                    if (LeftEvent != null)
                        LeftEvent(e.ObservedAbility);                                    
                }
                
                
            }
            _Within = entitys.ToList();
           
        }

        private bool _Find(List<TurnBasedRPG.PhysicalAbility> _Within, TurnBasedRPG.PhysicalAbility entity)
        {
            return _Within.Find(ent => ent.ObservedAbility.Id == entity.ObservedAbility.Id) != null;
        }

        private bool _Remove(List<PhysicalAbility> within, Guid id)
        {            
            return within.RemoveAll(ent => ent.ObservedAbility.Id == id) > 0;
        }

        


		
	}
}
