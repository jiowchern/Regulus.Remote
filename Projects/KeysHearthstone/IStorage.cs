using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.KeysHearthstone
{
	interface IStorage
	{
		Remoting.Value<Data.Account> FindAccount(string name);

		Remoting.Value<bool> AddAccount(string name, string password);
	}
}
