using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Map : Samebest.Game.IFramework 
    {
        class EntityInfomation
        {
            public Entity Entity {get;set;}
            public Action Exit; 
        }
        Regulus.Utility.Poller<EntityInfomation> _EntityInfomations = new Utility.Poller<EntityInfomation>();
		System.DateTime _Time = System.DateTime.Now;

		public long Time {get { return (System.DateTime.Now - _Time).Ticks ; }}

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
            
            var infos = _EntityInfomations.Update();
            var entitys = (from info in infos select info.Entity).ToArray();

            _UpdateObservers(entitys);

			_UpdateMovers(entitys);
            
            return true;
        }

		

        class ObserverInfomation
        {
            public Guid             Id;
            public IObserveAbility  ObserveAbility;            
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
							 let observed = entity.FindAbility<IMoverAbility>()
							 where observed != null
							 select observed).ToArray();

			foreach (var ability in abilitys)
			{
				ability.Update(Time , new CollisionInformation() );
			}
						
		}

        void Samebest.Game.IFramework.Shutdown()
        {
            
        }
    }
}
