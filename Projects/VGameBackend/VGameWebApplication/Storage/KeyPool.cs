using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGameWebApplication.Storage
{
    class KeyPool
    {
        class Key
        {
            public string Account;
            public VGame.Project.FishHunter.Storage.IUser User;
            VGame.Project.FishHunter.Storage.Proxy _Proxy;

            public Key(Guid id, VGame.Project.FishHunter.Storage.Proxy proxy, string account)
            {
                _Proxy = proxy;
                Id = id;                
                this.User = _Proxy.SpawnUser(id.ToString());
                this.Account = account;
                _TimeCounter = new Regulus.Utility.TimeCounter();
            }


            public Guid Id { get; set; }
            Regulus.Utility.TimeCounter _TimeCounter;
            internal bool IsTimeUp()
            {
                return _TimeCounter.Second > 300;                
            }

            internal void ResetTimeUp()
            {
                _TimeCounter.Reset();
            }

            internal void Release()
            {
                _Proxy.UnspawnUser(Id.ToString());
            }
        }

        public KeyPool(VGame.Project.FishHunter.Storage.Proxy proxy)
        {
            _Client = proxy;
            _Users = new List<Key>();
        }
        VGame.Project.FishHunter.Storage.Proxy _Client;
        List<Key> _Users;
        internal Guid Find(string account)
        {
            var u = _Users.Find(user => user.Account == account);
            if(u != null)
            {
                u.ResetTimeUp();
                return u.Id;
            }
            return Guid.Empty;
        }

        internal Guid Build(string account)
        {
            var id = Guid.NewGuid();            
            _Users.Add(new Key(id , _Client , account));
            return id;
        }

        

        internal void Update()
        {

            List<Key> timeUps = new List<Key>();
            foreach(var user in _Users)
            {
                if(user.IsTimeUp())
                {
                    timeUps.Add(user);
                }
            }

            foreach(var timeup in timeUps)
            {
                _Remove(timeup);
            }
        }

        private void _Remove(Key timeup)
        {
            timeup.Release();
            _Users.Remove(timeup);
        }

        internal VGame.Project.FishHunter.Storage.IUser Find(Guid id)
        {
            var user = _Users.Find(u => u.Id == id);
            if (user != null)
                return user.User;
            return null;
        }

        internal void Remove(Guid id)
        {
            var user = _Users.Find(u => u.Id == id);
            _Remove(user);
        }
    }
}
