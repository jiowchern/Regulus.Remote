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
        
        public void Into(Entity entity, Action exit_map)
        {
            _EntityInfomations.Add(new EntityInfomation() { Entity = entity, Exit = exit_map }); 
            IObserveAbility oa = entity.FindAbility<IObserveAbility>(); 
            if (oa != null)
            {                
                _AddObserver(entity.Id , oa);
            }
        }

        private void _AddObserver(Guid guid,IObserveAbility oa)
        {
 	        _ObserverInfomations.Add( new ObserverInfomation() { Id = guid , ObserveAbility = oa});
        }


        List<IObservedAbility> _Lefts = new List<IObservedAbility>();
        public void Left(Entity entity)
        {
            IObservedAbility oa = entity.FindAbility<IObservedAbility>();
            if (oa != null)
            {
                _Lefts.Add(oa);
            }
            _RemoveObserver(entity.Id);
            _EntityInfomations.Remove(info => info.Entity == entity);
            
        }

        private void _RemoveObserver(Guid guid)
        {
 	        _ObserverInfomations.Remove( oi =>  oi.Id == guid);
            
        }

        void Samebest.Game.IFramework.Launch()
        {
            
        }

        bool Samebest.Game.IFramework.Update()
        {
            
            var infos = _EntityInfomations.Update();
            var entitys = (from info in infos select info.Entity).ToArray();

            _UpdateObservers(entitys);
            
            return true;
        }

        class ObserverInfomation
        {
            public Guid             Id;
            public IObserveAbility  ObserveAbility;            
        }

        Regulus.Utility.Poller<ObserverInfomation> _ObserverInfomations = new Utility.Poller<ObserverInfomation>();
        private void _UpdateObservers(Entity[] entitys)
        {
            var observeds = (from entity in entitys 
                            let observed = entity.FindAbility<IObservedAbility>()
                            where observed != null
                            select observed).ToArray() ;

            foreach(var info in  _ObserverInfomations.Update())
            {
                info.ObserveAbility.Update(observeds, _Lefts);
            }
            _Lefts.Clear();
        }

        void Samebest.Game.IFramework.Shutdown()
        {
            
        }
    }
}
