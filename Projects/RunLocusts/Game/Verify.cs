using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{


    class Verify : Regulus.Utility.IStage, IVerify
    {
        private Regulus.Remoting.ISoulBinder _Binder;

        public delegate void OnDone(Data.Record record);

        public event OnDone DoneEvent;

        public Verify(Regulus.Remoting.ISoulBinder binder)
        {            
            this._Binder = binder;
        }
        void Regulus.Utility.IStage.Enter()
        {
            _Binder.Bind<IVerify>(this);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Binder.Unbind<IVerify>(this);
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }

        Regulus.Remoting.Value<Data.VerifyResult> IVerify.Login(string account, string password)
        {
            DoneEvent(new Data.Record { Id = Guid.NewGuid() });
            return Data.VerifyResult.Success;
        }
    }
}
