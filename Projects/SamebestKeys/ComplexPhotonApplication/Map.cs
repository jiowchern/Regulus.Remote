using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class Map : Regulus.Utility.IUpdatable, IMapInfomation, IMap
    {
        Regulus.Remoting.Time _Time;

        public class EntityInfomation : Regulus.Physics.IQuadObject
        {
            public Guid Id { get; set; }

            PhysicalAbility _Physical;
            public PhysicalAbility Physical 
            {
                get { return _Physical; }
                set 
                {
                    _Physical = value;
                    if (_Physical != null)
                        _Physical.BoundsChanged += _Physical_BoundsChanged;
                } 
            }

            void _Physical_BoundsChanged(object sender, EventArgs e)
            {
                _BoundsChanged.Invoke(this , e);
            }
            public IObservedAbility Observed { get; set; }
            public IMoverAbility2 Move { get; set; }
            public IObserveAbility Observe { get; set; }
            public ICrossAbility Cross { get; set; }

            Regulus.Types.Rect Physics.IQuadObject.Bounds
            {
                get { return Physical.Bounds; }
            }

            event EventHandler _BoundsChanged;
            event EventHandler Physics.IQuadObject.BoundsChanged
            {
                add { _BoundsChanged += value; }
                remove { _BoundsChanged -= value; }
            }
        }
					
        Regulus.Utility.Poller<EntityInfomation> _EntityInfomations = new Utility.Poller<EntityInfomation>();
        Regulus.Physics.QuadTree<EntityInfomation> _ObseverdInfomations;
        
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

            var ei = new EntityInfomation() 
            { 
                Id = entity.Id,                
                Physical = entity.FindAbility<PhysicalAbility>(),
                Observe = entity.FindAbility<IObserveAbility>(),
                Observed = entity.FindAbility<IObservedAbility>(),
                Move = entity.FindAbility<IMoverAbility2>(),
                Cross = entity.FindAbility<ICrossAbility>()
            };
			_EntityInfomations.Add(ei);

            if (ei.Physical != null)
            {
                _ObseverdInfomations.Insert(ei);
            }
			
        }

        List<IObservedAbility> _Lefts = new List<IObservedAbility>();
        public void Left(Entity entity)
        {
            
            _EntityInfomations.Remove((info) => 
            {
                if (info.Id == entity.Id)
                {
                    _ObseverdInfomations.Remove(info);
                    _Lefts.Add(info.Observed);
                    return true;
                }
                return false;
            });
			

            
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _ObseverdInfomations = new Physics.QuadTree<EntityInfomation>(new Regulus.Types.Size(4, 4), 0);
			_Build(_ReadMapData(Name));
        }

		private void _Build(Data.Map map)
		{
            if (map != null && map.Name == Name)
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

            Data.Map data = GameData.Instance.FindMap(name);
            return data;
		}


		bool Regulus.Utility.IUpdatable.Update()
        {
            _Time.Update();            
            var infos = _EntityInfomations.UpdateSet();

            _UpdateObservers(infos);

            _UpdateMovers(infos);
            
            return true;
        }

        

        private void _UpdateObservers(List<EntityInfomation> infos)
        {
            foreach (var info in infos)
            {
                var observer = info.Observe;
                if (observer != null)
                {
                    var brounds = observer.Vision;
                    var inbrounds = _ObseverdInfomations.Query(brounds);
                    observer.Update(inbrounds.ToArray(), _Lefts);                    
                }                
            }
            _Lefts.Clear();
        }

        
        
        private void _UpdateMovers(List<EntityInfomation> infos)
        {
            foreach (var info in infos)
            {
                var moverAbility = info.Move;
                var observeAbility = info.Observe;
                var physical = info.Physical;
                if (moverAbility != null && observeAbility != null && physical != null)
                {

                    var w = physical.Bounds.Width;
                    var h = physical.Bounds.Height;
                    var x = observeAbility.Position.X - w / 2;
                    var y = observeAbility.Position.Y - h / 2;
                    var brounds = new Regulus.Types.Rect(x, y, w, h);
                    var inbrounds = _ObseverdInfomations.Query(brounds);
                    var obbs = from qtoa in inbrounds let ma = qtoa.Move where ma != null && moverAbility != ma select ma.Polygon;
                    moverAbility.Update(_Time.Ticks, obbs);
                }
            }
        }


		void Regulus.Framework.ILaunched.Shutdown()
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
