using System;
using System.Collections.Generic;


namespace Regulus.Game
{    

   
    public class Entity 
    {
        

        enum COMMAND
        {
            ADD,REMOVE
        }
        struct Command
        {
            public COMMAND Cmd;
            public IComponment Com;
        }

        private readonly Dictionary<Type, List<IComponment>> _Componments;
        private readonly Queue<Command> _Commands;
        private readonly List<IComponment> _Updates;
        


        public Entity()
        {
            
            _Updates = new List<IComponment>();
            _Commands  = new Queue<Command>();
            _Componments = new Dictionary<Type, List<IComponment>>();
        }

        public void Destroy()
        {
        
            foreach (var componment in _Updates)
            {
                componment.End();
            }
            _Updates.Clear();
        }

        

        public void Update()
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
                _Componments.Add(type, newSet);
                newSet.Add(componment);
            }


            _Commands.Enqueue(new Command() { Cmd = COMMAND.ADD, Com = componment });
            
                    
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

        
    }
    
}