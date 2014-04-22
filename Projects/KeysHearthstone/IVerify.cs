using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.KeysHearthstone
{
	public interface IVerify
	{
		Remoting.Value<bool> Login(string name , string password);
		Remoting.Value<bool> CreateAccount(string name, string password);
	}
}
