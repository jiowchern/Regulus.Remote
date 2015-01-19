using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessServer
{
    public interface VerifyAPI
    {
        Regulus.Remoting.Value<bool> Login(string account , string password);
    }


    class Verify : Regulus.Utility.IStage, VerifyAPI
    {
        public event OnVerifySucess DoneEvent;
        private Regulus.Remoting.ISoulBinder _Binder;
        Storage _Storage;
        public Verify(Regulus.Remoting.ISoulBinder binder , Storage storage)
        {
            _Storage = storage;
            this._Binder = binder;
        }

        void Regulus.Utility.IStage.Enter()
        {
            _Binder.Bind<VerifyAPI>(this);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Binder.Unbind<VerifyAPI>(this);
        }

        void Regulus.Utility.IStage.Update()
        {            

        }

        Regulus.Remoting.Value<bool> VerifyAPI.Login(string name, string password)
        {
            Account account = _Storage.FindAccount(name, password);
            if(account != null)
            {
                DoneEvent(account.Id);
                return true;
            }
            return false;
        }
    }
}
