using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{
    
    class Kerb : Regulus.Utility.IStage, IKerb, Regulus.Utility.Coroutine.IUpdateable
    {
        private Regulus.Remoting.ISoulBinder _Binder;

        Regulus.Utility.Coroutine.Coroutine _Coroutine;

        public delegate void OnDone();
        public event OnDone DoneEvent;

        public Kerb(Regulus.Remoting.ISoulBinder binder)
        {            
            this._Binder = binder;
            _Coroutine = new Regulus.Utility.Coroutine.Coroutine();
        }


        void Regulus.Utility.IStage.Enter()
        {
            _Coroutine.Add(this);       
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Coroutine.Shutdown();
        }

        void Regulus.Utility.IStage.Update()
        {
            _Coroutine.Update();
        }

        IEnumerator<Regulus.Utility.Coroutine.INext> Regulus.Utility.Coroutine.IUpdateable.Enumerator()
        {
            var actorData = new ActorData();
            _Binder.Bind<IActorData>(actorData);
            yield return new Regulus.Utility.Coroutine.WaitOneFrame();
            _Binder.Unbind<IActorData>(actorData);

            DoneEvent();
            
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
