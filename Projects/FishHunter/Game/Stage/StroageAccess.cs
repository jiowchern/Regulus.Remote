using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Stage
{
    public class StroageAccess : Regulus.Utility.IStage, VGame.Project.FishHunter.IQuitable
    {

        public delegate void DoneCallback();
        public event DoneCallback DoneEvent;
        private Regulus.Remoting.ISoulBinder _Binder;
        private Data.Account _Account;
        private IStorage _Storage;

        public StroageAccess(Regulus.Remoting.ISoulBinder binder, Data.Account account, IStorage storage)
        {            
            this._Binder = binder;
            this._Account = account;
            this._Storage = storage;
        }

        void Regulus.Utility.IStage.Enter()
        {
            _Attach(_Account);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Detach(_Account);
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }

        void IQuitable.Quit()
        {
            DoneEvent();
        }

        private void _Attach(Data.Account account)
        {
            if ( account.HasCompetnce( Data.Account.COMPETENCE.ACCOUNT_FINDER) )
                _Binder.Bind<IAccountFinder>(_Storage);
            if (account.HasCompetnce(Data.Account.COMPETENCE.ACCOUNT_MANAGER))
                _Binder.Bind<IAccountManager>(_Storage);
        }
        private void _Detach(Data.Account account)
        {
            if (account.HasCompetnce(Data.Account.COMPETENCE.ACCOUNT_FINDER) )
                _Binder.Unbind<IAccountFinder>(_Storage);
            if (account.HasCompetnce(Data.Account.COMPETENCE.ACCOUNT_MANAGER) )
                _Binder.Unbind<IAccountManager>(_Storage);
        }


    }
}
