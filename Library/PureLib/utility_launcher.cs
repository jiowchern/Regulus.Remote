using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class Launcher 
    {
        List<Regulus.Framework.IBootable> _Launchers;

        public Launcher ()
        {
            _Launchers = new List<Framework.IBootable>();
        }

        public void Push(Regulus.Framework.IBootable laucnher)
        {
            _Launchers.Add(laucnher);
        }
        public void Shutdown()
        {
            foreach (var l in _Launchers)
                l.Shutdown();
        }

        public void Launch()
        {
            foreach (var l in _Launchers)
                l.Launch();
        }
    }
}
