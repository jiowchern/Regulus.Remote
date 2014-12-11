using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    class VerifyStage:Regulus.Game.IStage , IVerify
    {
        private Remoting.ISoulBinder _Binder;
        public delegate void DoneCallback(Account account);
        public event DoneCallback DoneEvent;
        IStorage _Storage;
        
        public VerifyStage(Remoting.ISoulBinder binder , IStorage storage)
        {            
            this._Binder = binder;
            _Storage = storage;
        }

        void Regulus.Game.IStage.Enter()
        {
            
        }

        void Regulus.Game.IStage.Leave()
        {
            
        }

        void Regulus.Game.IStage.Update()
        {
            
        }

        Remoting.Value<VerifyResult> IVerify.Login(string account, string password)
        {

            Account? accountData = _Storage.FindAccount(account);
            if (accountData.HasValue)
            {
                if (accountData.Value.Password == password)
                {
                    DoneEvent(accountData.Value);
                    return VerifyResult.Success;
                }
            }
            return VerifyResult.Fail;
        }
    }
}
