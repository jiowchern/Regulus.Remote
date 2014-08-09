using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Session
{
    class Staff
    {

        public delegate void OnBreak ();
        public event OnBreak BreakEvent;
    }
}
