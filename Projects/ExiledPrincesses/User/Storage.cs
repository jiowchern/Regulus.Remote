using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Standalone
{
	class Storage : Regulus.Project.ExiledPrincesses.IStorage
	{
        
        public Storage()
        { 

        }
		Regulus.Remoting.Value<AccountInfomation> IStorage.FindAccountInfomation(string name)
		{
			return new AccountInfomation() { Id = Guid.Empty , Name = name , Password = "1" };
		}

		void IStorage.Add(AccountInfomation ai)
		{
			
		}

        
    }
}

