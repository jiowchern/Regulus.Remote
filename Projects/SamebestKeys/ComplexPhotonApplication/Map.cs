using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class Map : Regulus.Utility.IUpdatable, IMapInfomation, IMap
    {
        

        public class EntityInfomation : Regulus.Physics.IQuadObject
        {
            public Guid Id { get; set; }

            PhysicalAbility _Physical;
            public PhysicalAbility Physical 
            {
                get { return _Physical; }
                set 
                {
                    if (value != null)
                        value.BoundsChanged -= _Physical_BoundsChanged;
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
            public IMoverAbility Move { get; set; }
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
        Regulus.Remoting.Time _Time;
        Regulus.Utility.Poller<EntityInfomation> _EntityInfomations = new Utility.Poller<EntityInfomation>();
        Regulus.Utility.Poller<EntityInfomation> _Observes = new Utility.Poller<EntityInfomation>();
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
        void _Into(Entity entity)
        {

            var ei = new EntityInfomation() 
            { 
                Id = entity.Id,                
                Physical = entity.FindAbility<PhysicalAbility>(),
                Observe = entity.FindAbility<IObserveAbility>(),
                Observed = entity.FindAbility<IObservedAbility>(),
                Move = entity.FindAbility<IMoverAbility>(),
                Cross = entity.FindAbility<ICrossAbility>()
            };
                        
			_EntityInfomations.Add(ei);

            if (ei.Observe != null)
            {
                _Observes.Add(ei);
            }

            if (ei.Physical != null)
            {
                _ObseverdInfomations.Insert(ei);
            }
			
        }

        List<IObservedAbility> _Lefts = new List<IObservedAbility>();
        void _Left(Entity entity)
        {
            
            _EntityInfomations.Remove((info) => 
            {
                if (info.Id == entity.Id)
                {                    
                    _ObseverdInfomations.Remove(info);
                    _RemoveObserve(info);
                    _Lefts.Add(info.Observed);
                    return true;
                }
                return false;
            });

            
        }

        private void _RemoveObserve(EntityInfomation info)
        {
            _Observes.Remove(i =>  i.Id == info.Id);
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
					_Into(e);					 
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
            _EntityInfomations.UpdateSet();
            var infos = _Observes.UpdateSet();


            foreach (var info in infos)
            {
                var observer = info.Observe;
                if (observer != null)
                {
                    _UpdateObservers(observer);
                }

                var moverAbility = info.Move;
                var observeAbility = info.Observe;
                var physical = info.Physical;
                if (moverAbility != null && observeAbility != null && physical != null)
                {

                    _UpdateMovers(moverAbility, observeAbility, physical);
                }
            }
           
            _Lefts.Clear();
            return true;
        }

        private void _UpdateMovers(IMoverAbility moverAbility, IObserveAbility observeAbility, PhysicalAbility physical)
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

        private void _UpdateObservers(IObserveAbility observer)
        {
            var brounds = observer.Vision;
            var inbrounds = _ObseverdInfomations.Query(brounds);
            observer.Update(inbrounds.ToArray(), _Lefts);
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

        Remoting.Value<Types.Polygon[]> IMapInfomation.QueryWalls()
        {
            return (from e in _EntityInfomations.Objects where e.Move != null select e.Move.Polygon).ToArray();
        }

        void IMap.Into(Entity entity)
        {
            _Into(entity);
        }

        void IMap.Left(Entity entity)
        {
            _Left(entity);
        }

        IMapInfomation IMap.GetInfomation()
        {
            return this;
        }
    }
}
