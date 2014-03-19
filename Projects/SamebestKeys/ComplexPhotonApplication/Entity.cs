using System;
using System.Collections.Generic;

namespace Regulus.Project.SamebestKeys
{

	interface IEntity
	{
		Guid Id { get; }       		 
	}
	abstract class Entity : IEntity
    {
		/// <summary>
		/// 功能Dict
		/// </summary>
        protected class AbilitySet
        {
            Dictionary<Type, object> _Abilitys = new Dictionary<Type, object>();

			/// <summary>
			/// 加入功能
			/// </summary>
            public void AttechAbility<T>(T ability)
            {
                _Abilitys.Add(typeof(T), ability as object);
            }

			/// <summary>
			/// 移除功能
			/// </summary>
            public void DetechAbility<T>()
            {
                _Abilitys.Remove(typeof(T));
            }

			/// <summary>
			/// 尋找功能
			/// </summary>
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
		/// <summary>
		/// 現有功能Dict
		/// </summary>
        AbilitySet _Abilitys = new AbilitySet();

		/// <summary>
		/// 尋找功能
		/// </summary>
        public T FindAbility<T>() where T : class
        {
            return _Abilitys.FindAbility<T>();
        }

		/// <summary>
		/// 設定功能
		/// </summary>
		/// <param name="abilitys">現有功能Dict</param>
        protected abstract void _SetAbility(AbilitySet abilitys);
        public void Initial()
        {
            _SetAbility(_Abilitys);
        }

		/// <summary>
		/// 移除功能
		/// </summary>
		/// <param name="abilitys">現有功能Dict</param>
        protected abstract void _RiseAbility(AbilitySet abilitys);
        public void Release()
        {
            _RiseAbility(_Abilitys);
        }
    }
}
