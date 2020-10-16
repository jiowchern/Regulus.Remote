using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Client
{
    

    public class AgentCommandBinder 
    {

        private readonly Type[] _WatchTypes;        
        readonly List<RectifierBinder> _Users;

        readonly Regulus.Remote.Client.AgentCommandRegister _Register;
        
        public AgentCommandBinder(AgentCommandRegister register ,  IEnumerable<Type> watch_types) 
        {
            _WatchTypes = watch_types.Union(new Type[0] ).ToArray();            
            _Users = new List<RectifierBinder>();
            _Register = register;
        }

        public System.Guid Bind(INotifierQueryable notifier)
        {
            AgentEventRectifier rectifier = new AgentEventRectifier(_WatchTypes, notifier);
            RectifierBinder user = new RectifierBinder(rectifier);
            _Users.Add(user);            
            foreach (Tuple<Type, object> g in user.Ghosts)
            {
                _Register.Regist(g.Item1, g.Item2);
            }
            rectifier.SupplyEvent += _Register.Regist;
            rectifier.UnsupplyEvent += (type, obj) => { _Register.Unregist(obj); };

            return user.Id;
        }
        public void Unbind(System.Guid id)
        {
            RectifierBinder user = _Users.FirstOrDefault((u) => u.Id == id);
            if (user == null)
                return;
            foreach (Tuple<Type, object> g in user.Ghosts)
            {
                _Register.Unregist(g.Item2);
            }
            _Users.RemoveAll(u => u.Id == id);            
            user.Dispose();
        }
        


    }
}