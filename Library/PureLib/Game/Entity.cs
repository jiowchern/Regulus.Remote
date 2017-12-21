using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Regulus.Framework;
using Regulus.Utility;

namespace Regulus.Game
{
    public class EntityUpdater : Launcher<IUpdatable>
    {
        internal IEnumerable<IUpdatable> Entities
        {
            get { return base._Objects; }
        }

        public void Update()
        {
            foreach (var updatable in _GetObjectSet())
            {
                if(updatable.Update() == false)
                    Remove(updatable);
            }
        }
    }
    public class EntitySet
    {
        private readonly EntityUpdater _Entitys;

        public IEnumerable<Entity> Entities { get { return from e in _Entitys.Entities select e as Entity; } }
        public EntitySet()
        {
            _Entitys = new EntityUpdater();
        }
        public Entity Create()
        {
            var entity = new Entity();
            _Entitys.Add(entity);
            return entity;
        }


        public void Update()
        {
            _Entitys.Update();
        }
    }

   
    public class Entity : IUpdatable
    {

        delegate void CommandDelegate();

        private readonly Queue<CommandDelegate> _Commands;
        private readonly Dictionary<System.Type, ComponmentSet> _ComponmentSets;
        private bool _Enable;

        public Entity()
        {
            
            _Commands = new Queue<CommandDelegate>();
            _ComponmentSets = new Dictionary<Type, ComponmentSet>();
        }

        public void Destroy()
        {
            _Enable = false;
        }
        bool IUpdatable.Update()
        {
            while (_Commands.Count > 0)
            {
                _Commands.Dequeue().Invoke();
            }

            foreach (var componmentSetsValue in _ComponmentSets.Values)
            {
                componmentSetsValue.Update(this);
            }
            return _Enable ;
        }

        public void Add<T>(T componment) where T : IComponment
        {
            _Commands.Enqueue(() =>
                {
                    var type = typeof(T);
                    ComponmentSet set;
                    if (_ComponmentSets.TryGetValue(type, out set))
                    {
                        set.Add<T>(componment);
                    }
                    else
                    {
                        var newSet = new ComponmentSet();
                        _ComponmentSets.Add(type, newSet);
                        newSet.Add(componment);
                    }

                });
            

        }

        public void Remove<T>(T componment) where T : IComponment
        {
            _Commands.Enqueue(() =>
            {
                var type = typeof(T);
                ComponmentSet set;
                if (_ComponmentSets.TryGetValue(type, out set))
                {
                    set.Remove<T>(componment);
                }
            });
            
        }

        

        public IEnumerable<T> Get<T>() where T : IComponment
        {
            var type = typeof(T);
            ComponmentSet set;
            if (_ComponmentSets.TryGetValue(type, out set))
            {
                return set.Get<T>();
            }

            
            return new T[0];
        }

        void IBootable.Launch()
        {
            _Enable = true;
        }

        void IBootable.Shutdown()
        {
            while (_Commands.Count > 0)
            {
                _Commands.Dequeue().Invoke();
            }

            foreach (var componmentSetsValue in _ComponmentSets.Values)
            {
                componmentSetsValue.Clear();
            }

            _ComponmentSets.Clear();
        }
    }
    
}