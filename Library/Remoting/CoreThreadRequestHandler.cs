using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Soul.Native
{
    
    public class CoreThreadRequestHandler : Regulus.Utility.IUpdatable 
    {
        Regulus.Remoting.IRequestQueue _Requester;
        bool _Enable;
        
        public CoreThreadRequestHandler(Regulus.Remoting.IRequestQueue requester)
        {
            _Requester = requester;
        }

        bool Utility.IUpdatable.Update()
        {
            _Requester.Update();
            return _Enable;
        }

        void Framework.IBootable.Launch()
        {
            _Enable = true;
            _Requester.BreakEvent += _End;
        }

        private void _End()
        {
            _Enable = false;
        }

        void Framework.IBootable.Shutdown()
        {
            _Requester.BreakEvent -= _End;
        }
    }
}
