using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class ObservedAbilityEqualityComparer : IEqualityComparer<Regulus.Project.SamebestKeys.Map.EntityInfomation>
    {


        bool IEqualityComparer<Map.EntityInfomation>.Equals(Map.EntityInfomation x, Map.EntityInfomation y)
        {
            return x.Id == y.Id;
        }

        int IEqualityComparer<Map.EntityInfomation>.GetHashCode(Map.EntityInfomation obj)
        {
            return obj.GetHashCode();
        }
    }
    class Field
    {


        List<Regulus.Project.SamebestKeys.Map.EntityInfomation> _Within = new List<Regulus.Project.SamebestKeys.Map.EntityInfomation>();

        public event Action<IObservedAbility> IntoEvent;
        public event Action<IObservedAbility> LeftEvent;
        internal void Update(IObserveAbility observe, Regulus.Project.SamebestKeys.Map.EntityInfomation[] entitys, List<IObservedAbility> lefts)
        {
            
            foreach (var exit in lefts)
            {
                _Remove(_Within, exit.Id);
                if (LeftEvent != null)
                    LeftEvent(exit);                
                
            }

            foreach(var e in entitys.Except(_Within))
            {
                if (e.Observed != null)
                {
                    if (IntoEvent != null)
                        IntoEvent(e.Observed);
                }
                
            }
            foreach (var e in _Within.Except(entitys, new ObservedAbilityEqualityComparer() ))
            {

                if (e.Observed != null)
                {
                    if (LeftEvent != null)
                        LeftEvent(e.Observed);
                }


            }
            _Within = entitys.ToList();
           
        }

        private bool _Find(List<Regulus.Project.SamebestKeys.Map.EntityInfomation> _Within, Regulus.Project.SamebestKeys.Map.EntityInfomation entity)
        {
            return _Within.Find(ent => ent.Id == entity.Id) != null;
        }

        private bool _Remove(List<Regulus.Project.SamebestKeys.Map.EntityInfomation> within, Guid id)
        {            
            return within.RemoveAll(ent => ent.Id == id) > 0;
        }

        internal void Clear()
        {
            foreach(var e in _Within)
            {
                if (e.Observed != null)
                {
                    if (LeftEvent != null)
                        LeftEvent(e.Observed);
                }
            }

            _Within.Clear();
        }
    }
}
