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
        private readonly Dictionary<Type, List<IComponment>> _Componments;

        enum COMMAND
        {
            ADD,REMOVE
        }
        struct Command
        {
            public COMMAND Cmd;
            public IComponment Com;
        }

        private readonly Queue<Command> _Commands;
        private List<IComponment> _Updates;

        private bool _Enable;

        public Entity()
        {
            _Updates = new List<IComponment>();
            _Commands  = new Queue<Command>();
            _Componments = new Dictionary<Type, List<IComponment>>();
        }

        public void Destroy()
        {
            _Enable = false;
        }
        bool IUpdatable.Update()
        {
            while (_Commands.Count > 0)
            {
                var cmd = _Commands.Dequeue();
                if (cmd.Cmd == COMMAND.ADD)
                {
                    _Updates.Add(cmd.Com);
                    cmd.Com.Start();
                }                    
                else
                {
                    
                    _Updates.Remove(cmd.Com);
                    cmd.Com.End();
                }
            }

            foreach (var componment in _Updates)
            {
                componment.Update();
            }
            
            return _Enable ;
        }

        
        public void Add<T>(T componment) where T : IComponment
        {
            var type = typeof(T);
            List<IComponment> set;
            if (_Componments.TryGetValue(type, out set))
            {
                set.Add(componment);
            }
            else
            {
                var newSet = new List<IComponment>();
                _Componments.Add(type , newSet);
                newSet.Add(componment);
            }


            _Commands.Enqueue(new Command(){ Cmd = COMMAND.ADD , Com = componment});
        }

        public void Remove<T>(T componment) where T : IComponment
        {
            var type = typeof(T);
            List<IComponment> set;
            if (_Componments.TryGetValue(type, out set))
            {
                set.Remove(componment);
            }

            _Commands.Enqueue(new Command() { Cmd = COMMAND.REMOVE, Com = componment });
        }

        

        public IEnumerable<T> Get<T>() where T : IComponment
        {
            var type = typeof(T);
            List<IComponment> componments;
            if (_Componments.TryGetValue(type, out componments))
            {
                foreach (var componment in componments)
                {
                    yield return (T)componment;
                }
            }            
        }

        void IBootable.Launch()
        {
            _Enable = true;
        }

        void IBootable.Shutdown()
        {

            foreach (var componment in _Updates)
            {
                componment.End();
            }

        }
    }
    
}