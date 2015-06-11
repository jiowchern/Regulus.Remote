using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Extension;
using System.Threading.Tasks;
namespace VGame.Project.FishHunter.Storage
{



    class Service
    {
        public IAccountManager AccountManager { get; private set; }
        
        public IAccountFinder AccountFinder { get; private set; }
        
        public IRecordQueriers RecodeQueriers { get; private set; }

        public ITradeNotes TradeNotes { get; private set; }


        System.Threading.Tasks.Task _ProxyUpdate;

        VGame.Project.FishHunter.Storage.Proxy _Proxy;

        volatile bool _Enable;

        Regulus.CustomType.Flag<VGame.Project.FishHunter.Data.Account.COMPETENCE> _Competnces;

        public Guid ConnecterId { get; private set; }


        public bool Enable {  get {return _Enable;}}
        
        Regulus.Utility.SpinWait _Soin;

        private VGameWebApplication.Models.VerifyData _Data;

        private IUser _User;

        object _Key;

        public Service(VGameWebApplication.Models.VerifyData data)
        {
            _Key = new object();
            this._Data = data;
            _Soin = new Regulus.Utility.SpinWait();
            _Enable = true;
            
            _Proxy = new VGame.Project.FishHunter.Storage.Proxy();
            (_Proxy as Regulus.Utility.IUpdatable).Launch();
            _ProxyUpdate = new System.Threading.Tasks.Task(_UpdateProxy, new WeakReference<Regulus.Utility.IUpdatable>(_Proxy));
            _ProxyUpdate.Start();

            _User = _Proxy.SpawnUser("1");
        }
       
        ~Service()
        {
            lock (_Proxy)
                (_Proxy as Regulus.Utility.IUpdatable).Shutdown();
        }

        bool _Initial()
        {
            if (_Connect())
            {
                if (_Verify())
                {
                    _GetStorageCompetnces();

                    if (_Competnces[Data.Account.COMPETENCE.ACCOUNT_MANAGER])
                        _GetAccountManager();

                    if (_Competnces[Data.Account.COMPETENCE.ACCOUNT_FINDER])
                        _GetAccountFinder();

                    _GetAllAccountRecode();
                    return true;
                }
                else
                    return false;
            }
            else
            {
                throw new SystemException("storage verify fail.");
            }
        }

        private bool _Connect()
        {
            while (_User.Remoting.ConnectProvider.Ghosts.Length <= 0)
                _Wait();

            return _User.Remoting.ConnectProvider.Ghosts[0].Connect("127.0.0.1", 38973).WaitResult();
        }

        private bool _Verify()
        {
            while (_User.VerifyProvider.Ghosts.Length <= 0)
                _Wait();

            return _User.VerifyProvider.Ghosts[0].Login(_Data.Account, _Data.Password).WaitResult();
        }

        private void _GetStorageCompetnces()
        {
            var provider = _User.QueryProvider<IStorageCompetences>();
            while (provider.Ghosts.Length <= 0)
                _Wait();

            _Competnces = new Regulus.CustomType.Flag<Data.Account.COMPETENCE>(provider.Ghosts[0].Query().WaitResult());

            ConnecterId = provider.Ghosts[0].QueryForId().WaitResult();
        }

        private void _GetAccountManager()
        {
            var provider = _User.QueryProvider<IAccountManager>();
            while (provider.Ghosts.Length <= 0)
                _Wait();

            AccountManager = provider.Ghosts[0];
        }

        private void _GetAccountFinder()
        {
            var provider = _User.QueryProvider<IAccountFinder>();
            while (provider.Ghosts.Length <= 0)
                _Wait();

            AccountFinder = provider.Ghosts[0];
        }

        private void _GetAllAccountRecode()
        {
            //var accounts = AccountManager.QueryAllAccount().WaitResult();

            var provider = _User.QueryProvider<IRecordQueriers>();
            while (provider.Ghosts.Length <= 0)
                _Wait();
            
            RecodeQueriers = provider.Ghosts[0];

            
            var p = _User.QueryProvider<ITradeNotes>();
            while (p.Ghosts.Length <= 0)
                _Wait();
            TradeNotes = p.Ghosts[0];
            
            //int money = provider.Ghosts[0].Load(accounts[0].Id).WaitResult().Money;
        }

        
        public void Release()
        {            
        }
       

        private void _Wait()
        {
            _Soin.SpinOnce();
        }

        static private void _UpdateProxy(object obj)
        {
            WeakReference<Regulus.Utility.IUpdatable> weak = (WeakReference<Regulus.Utility.IUpdatable>)obj;

            Regulus.Utility.SpinWait spin = new Regulus.Utility.SpinWait();

            bool enable = true;

            while (enable)
            {
                Regulus.Utility.IUpdatable updater;
                enable = weak.TryGetTarget(out updater);
                if (Regulus.Utility.Random.NextFloat(0, 1) <= 0.1f)
                    spin.SpinOnce();
                else
                    spin.Reset();

                lock (updater)
                    updater.Update();
            }
        }

        internal static Guid Verify(string user, string password)
        {
            
            var service = new Service(new VGameWebApplication.Models.VerifyData { Account = user, Password = password });
            if(service._Initial())
            {
                service.Release();
                return VGame.Project.FishHunter.Storage.KeyPool.Instance.Query(user, password);
            }
                
            return Guid.Empty;
        }

        internal static Service Create(Guid id)
        {
            var data = VGame.Project.FishHunter.Storage.KeyPool.Instance.Find(id);
            if(data != null)
            {
                var service = new Service(data);
                if(service._Initial())
                    return service;
            }
            return null;
        }

        internal static Service Create(object p)
        {
            return Create((Guid)p);
        }

        internal static void Destroy(Guid guid)
        {
            VGame.Project.FishHunter.Storage.KeyPool.Instance.Destroy(guid);
        }
    }
}
