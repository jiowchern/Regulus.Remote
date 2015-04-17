using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGameWebApplication.Models
{
    public class StorageApi
    {
        private Guid id;

        public StorageApi(Guid id)
        {            
            this.id = id;
        }
        public string Account { get { return VGame.Project.FishHunter.Storage.Appliction.Instance.GetAccount(id); } }
        public VGame.Project.FishHunter.IAccountManager AccountManager { get { return VGame.Project.FishHunter.Storage.Appliction.Instance.FindApi<VGame.Project.FishHunter.IAccountManager>(id); } }
        public VGame.Project.FishHunter.IAccountFinder AccountFinder { get { return VGame.Project.FishHunter.Storage.Appliction.Instance.FindApi<VGame.Project.FishHunter.IAccountFinder>(id); } }
    }
}
