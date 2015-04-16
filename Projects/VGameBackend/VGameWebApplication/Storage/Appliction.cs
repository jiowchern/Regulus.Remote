using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VGameWebApplication.Storage;

namespace VGame.Project.FishHunter.Storage
{
    public class Appliction : Regulus.Utility.Singleton<Appliction> 
    {
        KeyPool _Keys;
        Proxy _Proxy;
        public Appliction()
        {
            
            _Proxy = new Proxy();
            _Keys = new KeyPool(_Proxy);
            Task task = new Task(_Run);
            task.Start();
        }

        private void _Run()
        {
            var updater = new Regulus.Utility.Updater();
            System.Threading.SpinWait sw = new System.Threading.SpinWait();
            updater.Add(_Proxy);
            Enable = true;
            while(Enable)
            {
                sw.SpinOnce();
                updater.Update();
                _Keys.Update();
            }
            updater.Shutdown();
        }
        public async Task <Guid> Create(string ip,int port,string account ,string password)
        {

            Guid id = _Keys.Find(account);
            if(id == Guid.Empty)
            {
                id = _Keys.Build(account);
                var connectHandler = new VGameWebApplication.Storage.Handler.Connect(_Keys.Find(id), ip, port);
                await connectHandler.Handle();
                if(connectHandler.Result == false)
                {
                    _Keys.Remove(id);
                    return Guid.Empty;
                }
            }


            var verifyHandler = new VGameWebApplication.Storage.Handler.Verify(_Keys.Find(id), account, password);
            await verifyHandler.Handle();
            if (verifyHandler.Result)
                return id;
            
            return Guid.Empty;            
        }

        private async void _Verify(string account, string password, Guid id)
        {
            var handler = new VGameWebApplication.Storage.Handler.Verify(_Keys.Find(id), account, password);
            await handler.Handle();
        }

        private async void _Connect(string ip, int port, Guid id)
        {
            var handler = new VGameWebApplication.Storage.Handler.Connect(_Keys.Find(id), ip, port);
            bool connectResult = await handler.Handle();            
        }
        public volatile bool Enable;

        internal bool Valid(Guid guid)
        {
            return _Keys.Find(guid) != null;
        }

        internal void Destroy(Guid id)
        {
            _Keys.Remove(id);
        }

        internal T FindApi<T>(Guid id)
        {
            var key = _Keys.Find(id);   
            if(key  != null)
            {
                var ghosts = key.QueryProvider<T>().Ghosts;
                if (ghosts.Length == 1)
                {
                    return ghosts[0];
                }
                else if (ghosts.Length > 1)
                    throw new SystemException("api only one");
            }
            return default(T);
        }

        internal string GetAccount(Guid id)
        {
            return _Keys.FindAccount(id);               
        }
    }
}
