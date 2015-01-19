using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Dungeons
{
    class IdleStage : Regulus.Utility.IStage, IIdle
    {
        public delegate void OnGotoRealm(string realm);
        public event OnGotoRealm GotoRealmEvent;

        Remoting.ISoulBinder _Binder;
        public IdleStage(Remoting.ISoulBinder binder)
        {
            _Binder = binder;
        }
        void Utility.IStage.Enter()
        {
            _Binder.Bind<IIdle>(this);
        }

        void Utility.IStage.Leave()
        {
            _Binder.Unbind<IIdle>(this);
        }

        void Utility.IStage.Update()
        {
            
        }

        void IIdle.GotoRealm(string realm)
        {
            GotoRealmEvent(realm);
        }
    }
}
