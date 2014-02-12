using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

	interface IEntity
	{
		Guid Id { get; }       		 
	}
	abstract class Entity : IEntity
    {
        protected class AbilitySet
        {
            Dictionary<Type, object> _Abilitys = new Dictionary<Type, object>();
            public void AttechAbility<T>(T ability)
            {
                _Abilitys.Add(typeof(T), ability as object);
            }

            public void DetechAbility<T>()
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

        public Entity(Guid id )
        {
            Id = id;            
        }
        public Guid Id { get; private set; }        

        AbilitySet _Abilitys = new AbilitySet();
        public T FindAbility<T>() where T : class
        {
            return _Abilitys.FindAbility<T>();
        }

        protected abstract void _SetAbility(AbilitySet abilitys);
        public void Initial()
        {
            _SetAbility(_Abilitys);
        }
        protected abstract void _RiseAbility(AbilitySet abilitys);
        public void Release()
        {
            _RiseAbility(_Abilitys);
        }
    }
}
