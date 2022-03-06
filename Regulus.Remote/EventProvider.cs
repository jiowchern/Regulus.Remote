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
        private readonly IEventProxyCreater[] _ProxyCreaters;

        public EventProvider(IEnumerable<IEventProxyCreater> closures)
        {
            _ProxyCreaters = closures.ToArray();
        }


        private IEventProxyCreater _Find(EventInfo info)
        {
            return (from closure in _ProxyCreaters
                    where closure.GetType() == info.DeclaringType && closure.GetName() == info.Name
                    select closure).FirstOrDefault();
        }


        public IEventProxyCreater Find(EventInfo info)
        {
            return _Find(info);

        }
    }
}

