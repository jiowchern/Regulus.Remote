using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal.Standalone
{
	class Storage : Regulus.Project.Crystal.IStorage
	{
		Remoting.Value<AccountInfomation> IStorage.FindAccountInfomation(string name)
		{
			return new AccountInfomation() { Id = Guid.Empty , Name = name , Password = "1" };
		}

		void IStorage.Add(AccountInfomation ai)
		{
			
		}
	}
}
