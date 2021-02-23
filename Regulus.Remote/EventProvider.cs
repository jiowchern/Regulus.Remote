using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Sample
{
}
namespace Regulus.Remote
{
    public class EventProvider
    {
        private readonly IEventProxyCreator[] _ProxyCreators;

        public EventProvider(IEnumerable<IEventProxyCreator> closures)
        {
            _ProxyCreators = closures.ToArray();
        }


        private IEventProxyCreator _Find(EventInfo info)
        {
            return (from closure in _ProxyCreators
                    where closure.GetType() == info.DeclaringType && closure.GetName() == info.Name
                    select closure).FirstOrDefault();
        }


        public IEventProxyCreator Find(EventInfo info)
        {
            return _Find(info);

        }
    }
}

