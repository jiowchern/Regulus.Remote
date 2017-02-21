using System;
using System.Linq;
using System.Collections.Generic;

namespace Regulus.Remoting
{
    public class EventProvider
    {
        private readonly IEventProxyCreator[] _ProxyCreators;

        public EventProvider(IEnumerable<IEventProxyCreator> closures)
        {
            _ProxyCreators = closures.ToArray();
        }
        

        private IEventProxyCreator _Find(Type soul_type, string event_name)
        {
            return (from closure in _ProxyCreators
                    where closure.GetType() == soul_type && closure.GetName() == event_name
                    select closure).FirstOrDefault();            
        }


        public IEventProxyCreator Find(Type soul_type , string event_name)
        {            
            return  _Find(soul_type , event_name);
            
        }
    }
}

