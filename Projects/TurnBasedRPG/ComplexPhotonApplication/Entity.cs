using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Entity 
    {
        
        public Entity(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; private set; }

        Dictionary<Type, object> _Abilitys = new Dictionary<Type, object>();
        protected void _AttechAbility<T>(T ability)
        {
            _Abilitys.Add(typeof(T) , ability as object);
        }

        protected void _DetechAbility<T>()
        {
            _Abilitys.Remove(typeof(T));
        }

        public T FindAbility<T>() where T : class
        {
            object o;
            if (_Abilitys.TryGetValue(typeof(T), out o))
            {
                return o as T;
            }
            return default(T);
        }
    }
}
