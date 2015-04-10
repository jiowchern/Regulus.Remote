using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGame.Project.FishHunter.Storage
{
    public class Server : Regulus.Utility.ICore , IStorage
    {

        Regulus.Utility.Updater _Updater;
        VGame.Project.FishHunter.Storage.Center _Center;
        Regulus.Utility.ICore _Core { get { return _Center; } }
        Regulus.NoSQL.Database _Database;
        private string _Ip;
        private string _Name;
        private string _DefaultAdministratorName;

        public Server()
        {
            _DefaultAdministratorName = "vgameadmini";
            _Ip = "mongodb://127.0.0.1:27017";
            _Name = "VGame";
            _Updater = new Regulus.Utility.Updater();
            _Database = new Regulus.NoSQL.Database();
            _Center = new Center(this);
        }
        void Regulus.Utility.ICore.ObtainController(Regulus.Remoting.ISoulBinder binder)
        {
            _Core.ObtainController(binder);
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {

            
            _Updater.Add(_Center);
            _Database.Launch(_Ip, _Name);


            _HandleAdministrator();
        }

        private void _HandleAdministrator()
        {
            var account = (from a in _Database.Linq<Data.Account>() where a.Name == _DefaultAdministratorName select a).FirstOrDefault();
            if(account == null)
            {
                account = new Data.Account()
                {
                    Id = Guid.NewGuid(),
                    Name = _DefaultAdministratorName,
                    Password = "",
                    Competnce = Data.Account.COMPETENCE.ALL
                };

                _Database.Add(account);
            }
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Database.Shutdown();
            _Updater.Shutdown();
        }

        Regulus.Remoting.Value<Data.Account> IAccountFinder.FindAccount(string name)
        {
            var account = (from a in _Database.Linq<Data.Account>() where a.Name == name select a).FirstOrDefault();

            if (account != null)
                return account;

            return new Regulus.Remoting.Value<Data.Account>(null);
        }
    }
}
