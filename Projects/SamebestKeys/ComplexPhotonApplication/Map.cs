using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class Map : Regulus.Utility.IUpdatable, IMapInfomation, IMap 
    {        
        Guid _Id;
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
                if (_BoundsChanged != null)
                    _BoundsChanged.Invoke(this , e);
            }
            public IObservedAbility Observed { get; set; }
            public IMoverAbility Move { get; set; }
            public IObserveAbility Observe { get; set; }
            public ICrossAbility Cross { get; set; }
            public IBehaviorAbility Behavior { get; set; }
            public IBehaviorCommandAbility Commander { get; set; }
            public ISkillCaptureAbility Effect { get; set; }
            public IActorPropertyAbility Property { get; set; }
            public IActorUpdateAbility Update { get; set; }

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

            internal void Release()
            {
                _Physical.BoundsChanged -= _Physical_BoundsChanged;
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
				
		public Map(string name,Regulus.Remoting.ITime time)
		{
            _Id = Guid.NewGuid();
			_Name = name;
            _SetTime(time);
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
                Cross = entity.FindAbility<ICrossAbility>(),
                Behavior = entity.FindAbility<IBehaviorAbility>(),
                Commander = entity.FindAbility<IBehaviorCommandAbility>(),
                Effect = entity.FindAbility <ISkillCaptureAbility>(),
                Property = entity.FindAbility<IActorPropertyAbility>(),
                Update = entity.FindAbility<IActorUpdateAbility>(),
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
                    info.Release();
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
			_Build(_ReadMapData(_Name));
        }

		private void _Build(Data.Map map)
		{
            if (map != null && map.Name == _Name)
			{			
				foreach(var ent in map.Entitys)
				{
					var e= EntityBuilder.Instance.Build(ent);					
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
                var physical = info.Physical;
                if (moverAbility != null && observer != null && physical != null)
                {
                    _UpdateMovers(moverAbility, observer, physical);
                }

                var behavior = info.Behavior;   
                if (behavior != null)
                {
                    behavior.Update();
                }
                if (info.Effect != null && info.Property!= null)
                    _UpdateEffect(info);

                if (info.Update != null)
                    info.Update.Update();
            }

            if (_LeftDoneEvent != null)
                foreach(var l in _Lefts)
                {
                    _LeftDoneEvent(l.Id);                    
                }
            _Lefts.Clear();
            return true;
        }
        private void _UpdateEffect(EntityInfomation entity)
        {
            var behavior = entity.Effect;
            var property = entity.Property;
            var obs = entity.Observed;
            Types.Rect bounds = new Types.Rect();
            int skill = 0;            
            if (behavior.TryGetBounds(ref bounds , ref skill))
            {
                var s = GameData.Instance.FindSkill(skill);            
                if (s != null)
                {
                    if (s.Id == 1)
                    {
                        property.ChangeMode();
                        behavior.Hit();
                    }
                    else if (s.Id == 2 )
                    {
                        var inbrounds = _ObseverdInfomations.Query(bounds);
                        var commanders = from e1 in inbrounds where e1.Id != entity.Id && e1.Commander != null select e1.Commander;
                        foreach (var commander in commanders)
                        {
                            commander.Invoke(new BehaviorCommand.Injury(s.Param1, obs.Direction));
                        }
                        if (commanders.Count() > 0)
                        {
                            behavior.Hit();
                        }

                    }

                    
                }
                
            }
            
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
        
        
        void _SetTime(Regulus.Remoting.ITime time)
        {
            _Time = new Regulus.Remoting.Time(time);
        }

		string _Name { get; set; }

        Remoting.Value<Types.Polygon[]> IMapInfomation.QueryWalls()
        {
            return (from e in _EntityInfomations.UpdateSet() where e.Move != null select e.Move.Polygon).ToArray();
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

        Guid IMap.Id
        {
            get { return _Id; }
        }


        public string Name { get { return _Name; } }

        string IMap.Name
        {
            get
            {
                return _Name;
            }
            
        }

        event Action<Guid> _LeftDoneEvent;
        event Action<Guid> IMap.LeftDoneEvent
        {
            add { _LeftDoneEvent += value; }
            remove { _LeftDoneEvent -= value; }
        }
    }
}
