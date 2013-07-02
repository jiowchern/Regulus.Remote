using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal
{
    
    class Storage : IStorage
	{
		Samebest.NoSQL.Database _Database;
		internal void Initial()
		{
			_Database = new Samebest.NoSQL.Database();
            _Database.Launch("mongodb://127.0.0.1:27017" , "Crystal");
		}

		internal void Finial()
		{
			_Database.Shutdown();
		}

        Samebest.Remoting.Value<AccountInfomation> IStorage.FindAccountInfomation(string name)
        {
            var ais = _Database.Query<AccountInfomation>();
            var reault = (from a in ais where a.Name == name select a).FirstOrDefault();
            return reault;
        }

        void IStorage.Add(AccountInfomation ai)
        {
            _Database.Add(ai);
        }
    }
}
