using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Dungeons
{
    class IdleStage : Regulus.Game.IStage, IIdle
    {
        public delegate void OnGotoRealm(string realm);
        public event OnGotoRealm GotoRealmEvent;

        Remoting.ISoulBinder _Binder;
        public IdleStage(Remoting.ISoulBinder binder)
        {
            _Binder = binder;
        }
        void Game.IStage.Enter()
        {
            _Binder.Bind<IIdle>(this);
        }

        void Game.IStage.Leave()
        {
            _Binder.Unbind<IIdle>(this);
        }

        void Game.IStage.Update()
        {
            
        }

        void IIdle.GotoRealm(string realm)
        {
            GotoRealmEvent(realm);
        }
    }
}
