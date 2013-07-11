using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Map : Regulus.Game.IFramework , IMapInfomation
    {
        Regulus.Remoting.Time _Time;
        class EntityInfomation 
        {
            public Entity Entity {get;set;}
            public Action Exit; 
		}


		
			
        Regulus.Utility.Poller<EntityInfomation> _EntityInfomations = new Utility.Poller<EntityInfomation>();
		Regulus.Physics.QuadTree<Regulus.Physics.IQuadObject> _ObseverdInfomations;
		
		
		
        
		long _DeltaTime 
        {  
            get 
            {
                return _Time.Delta; 
            } 
        }

        public void Into(Entity entity, Action exit_map)
        {

			var qo = entity.FindAbility<Regulus.Physics.IQuadObject>();
			if (qo != null)
			{
				_ObseverdInfomations.Insert(qo);	
			}
			

			var ei = new EntityInfomation() { Entity = entity, Exit = exit_map };
			_EntityInfomations.Add(ei);    
			
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


			var qo = entity.FindAbility<Regulus.Physics.IQuadObject>();
			if (qo != null)
			{
				_ObseverdInfomations.Remove(qo);
			}
            
        }

        void Regulus.Game.IFramework.Launch()
        {
			_ObseverdInfomations = new Physics.QuadTree<Regulus.Physics.IQuadObject>(new System.Windows.Size(4, 4), 0);
        }

        bool Regulus.Game.IFramework.Update()
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
				//observer.Update(observeds, _Lefts);

				observer.Update(_ObseverdInfomations , _Lefts);
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

        void Regulus.Game.IFramework.Shutdown()
        {
            
        }
        
        
        internal void SetTime(Regulus.Remoting.ITime time)
        {
            _Time = new Regulus.Remoting.Time(time);
        }
    }
}
