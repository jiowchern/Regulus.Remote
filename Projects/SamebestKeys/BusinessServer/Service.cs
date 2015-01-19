using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessServer
{

    interface ServiceAPI
    {
        void PushCoins(string player_id, int coins, string remark);
        void Logout();
    }
    class Service : Regulus.Utility.IStage, ServiceAPI
    {
        private Regulus.Remoting.ISoulBinder _Binder;
        private Storage _Storage;

        public delegate void OnDone();
        public event OnDone DoneEvent;

        public int _Id;

        public Service(Regulus.Remoting.ISoulBinder binder, Storage storage , int account_id)
        {
            // TODO: Complete member initialization
            this._Binder = binder;
            this._Storage = storage;

            _Id = account_id;
        }

        public Service(Regulus.Remoting.ISoulBinder _Binder, Storage _Storage)
        {
            // TODO: Complete member initialization
            this._Binder = _Binder;
            this._Storage = _Storage;
        }

        void Regulus.Utility.IStage.Enter()
        {
            
        }

        void Regulus.Utility.IStage.Leave()
        {
            
        }

        void Regulus.Utility.IStage.Update()
        {
            
        }



        void ServiceAPI.PushCoins(string player_id, int coins ,string remark)
        {
            _Storage.PushCoins(_Id, player_id, coins, remark);
        }

        void ServiceAPI.Logout()
        {
            DoneEvent();
        }
    }
}
