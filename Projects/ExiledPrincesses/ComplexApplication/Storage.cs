using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses
{
    
    class Storage : IStorage
	{
		Regulus.NoSQL.Database _Database;
		internal void Initial()
		{
			_Database = new Regulus.NoSQL.Database();
            _Database.Launch("mongodb://127.0.0.1:27017" , "ExiledPrincesses");
		}

		internal void Finial()
		{
			_Database.Shutdown();
		}

        Regulus.Remoting.Value<AccountInfomation> IStorage.FindAccountInfomation(string name)
        {
            var ais = _Database.Query<AccountInfomation>();
            var reault = (from a in ais where a.Name == name select a).FirstOrDefault();
            return reault;
        }

        void IStorage.Add(AccountInfomation ai)
        {
            _Database.Add(ai);
        }



        void IStorage.Add(Pet pet)
        {
            _Database.Add(pet);
        }

        Remoting.Value<Pet> IStorage.FindPet(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
