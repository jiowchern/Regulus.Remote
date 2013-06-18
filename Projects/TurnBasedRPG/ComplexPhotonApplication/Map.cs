using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Map : Samebest.Game.IFramework , IMapInfomation
    {
        Samebest.Remoting.Time _Time;
        class EntityInfomation
        {
            public Entity Entity {get;set;}
            public Action Exit; 
        }
        Regulus.Utility.Poller<EntityInfomation> _EntityInfomations = new Utility.Poller<EntityInfomation>();
		
        
		long _DeltaTime 
        {  
            get 
            {
                return _Time.Delta; 
            } 
        }

        public void Into(Entity entity, Action exit_map)
        {
            _EntityInfomations.Add(new EntityInfomation() { Entity = entity, Exit = exit_map });         
        }

        List<IObservedAbility> _Lefts = new List<IObservedAbility>();
        public void Left(Entity entity)
        {
            IObservedAbility oa = entity.FindAbility<IObservedAbility>();
            if (oa != null)
            {
                _Lefts.Add(oa);
            }
            
            _EntityInfomations.Remove(info => info.Entity == entity);
            
        }

        void Samebest.Game.IFramework.Launch()
        {
            
        }

        bool Samebest.Game.IFramework.Update()
        {
            _Time.Update();            
            var infos = _EntityInfomations.UpdateSet();
            var entitys = (from info in infos select info.Entity).ToArray();

            _UpdateObservers(entitys);

			_UpdateMovers(entitys);
            
            return true;
        }

        
        private void _UpdateObservers(Entity[] entitys)
        {
            var observeds = (from entity in entitys 
                            let observed = entity.FindAbility<IObservedAbility>()
                            where observed != null
                            select observed).ToArray() ;
			var observers = from entity in entitys
							 let observed = entity.FindAbility<IObserveAbility>()
							 where observed != null
							 select observed;

			foreach (var observer in observers)
            {
				observer.Update(observeds, _Lefts);
            }
            _Lefts.Clear();
        }

		private void _UpdateMovers(Entity[] entitys)
		{
			var abilitys = (from entity in entitys
                            let ability = entity.FindAbility<IMoverAbility>()
                            where ability != null
                            select ability).ToArray();

			foreach (var ability in abilitys)
			{
                ability.Update(_Time.Ticks, new CollisionInformation());
			}
						
		}

        void Samebest.Game.IFramework.Shutdown()
        {
            
        }
        
        
        internal void SetTime(Samebest.Remoting.ITime time)
        {
            _Time = new Samebest.Remoting.Time(time);
        }
    }
}
