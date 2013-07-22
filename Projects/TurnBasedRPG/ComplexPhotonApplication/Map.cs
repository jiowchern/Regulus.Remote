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
            public QuadTreeObjectAbility Observed;
		}
					
        Regulus.Utility.Poller<EntityInfomation> _EntityInfomations = new Utility.Poller<EntityInfomation>();
        Regulus.Physics.QuadTree<QuadTreeObjectAbility> _ObseverdInfomations;
        
		long _DeltaTime 
        {  
            get 
            {
                return _Time.Delta; 
            } 
        }
				
		public Map(string name)
		{
			Name = name;
		}
        public void Into(Entity entity)
        {

            var qtoa = entity.FindAbility<QuadTreeObjectAbility>();
            var oa = entity.FindAbility<IObservedAbility>();

            if (qtoa != null)
			{
                _ObseverdInfomations.Insert(qtoa);	
			}

            var ei = new EntityInfomation() { Entity = entity, Observed = qtoa };
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
            _EntityInfomations.Remove((info) => 
            {
                if (info.Entity == entity)
                {
                    _ObseverdInfomations.Remove(info.Observed);
                    return true;
                }
                return false;
            });
			

            
        }

        void Regulus.Game.IFramework.Launch()
        {
            _ObseverdInfomations = new Physics.QuadTree<QuadTreeObjectAbility>(new System.Windows.Size(4, 4), 0);
			_Build(_ReadMapData(Name));
        }

		private void _Build(Data.Map map)
		{
			if(map.Name == Name)
			{			
				foreach(var ent in map.Entitys)
				{
					var e= EntityBuilder.Instance.Build(ent);
					e.Initial();
					Into(e);					 
				}
			}
		}

        private Data.Map _ReadMapData(string name)
		{
			string path = "../TrunBasedRPG/Complex/Data/" + Name + ".map";
			var data = Regulus.Utility.IO.Serialization.Read<Data.Map>(path);
			return data;
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
			var observers = from entity in entitys
							 let observed = entity.FindAbility<IObserveAbility>()
							 where observed != null
							 select observed;
            
			foreach (var observer in observers)
            {
                var w = observer.Vision;
                var h = observer.Vision;
                var x = observer.Observed.Position.X - observer.Vision / 2;
                var y = observer.Observed.Position.Y - observer.Vision / 2;
                var brounds = new System.Windows.Rect(x, y, w, h);
                var inbrounds = _ObseverdInfomations.Query(brounds);
                var observeds = (from inbround in inbrounds let observedAbility = inbround.ObservedAbility where observedAbility  != null select observedAbility).ToArray();
                observer.Update(observeds , _Lefts);
            }
            _Lefts.Clear();
        }

		private void _UpdateMovers(Entity[] entitys)
		{

            var movers = (from entity in entitys
                          let moverAbility = entity.FindAbility<IMoverAbility>()                          
                          let observeAbility = entity.FindAbility<IObserveAbility>()
                          let quadTreeObjectAbility = entity.FindAbility<QuadTreeObjectAbility>()
                          where moverAbility != null && observeAbility != null && quadTreeObjectAbility != null
                          select new { Mover = moverAbility, ObserveAbility = observeAbility, Qtoa = quadTreeObjectAbility }).ToArray();

            foreach (var mover in movers)
			{
                var w = mover.Qtoa.Bounds.Width;
                var h = mover.Qtoa.Bounds.Height;
                var x = mover.ObserveAbility.Observed.Position.X - w / 2;
                var y = mover.ObserveAbility.Observed.Position.Y - h / 2;
                var brounds = new System.Windows.Rect(x, y, w, h);
                var inbrounds = _ObseverdInfomations.Query(brounds);
                var obbs = from qtoa in inbrounds let ma = qtoa.MoverAbility where ma != null && mover.Mover != ma select ma.Obb;
                mover.Mover.Update(_Time.Ticks, obbs);
                
			}
						
		}

        void Regulus.Game.IFramework.Shutdown()
        {            
            _ObseverdInfomations = null;
        }
        
        
        internal void SetTime(Regulus.Remoting.ITime time)
        {
            _Time = new Regulus.Remoting.Time(time);
        }

		public string Name { get; private set; }
	}
}
