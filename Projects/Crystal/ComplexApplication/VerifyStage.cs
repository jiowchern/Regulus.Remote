using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal
{
	class VerifyStage : Samebest.Game.IStage<User> , IVerify
	{
		
		void Samebest.Game.IStage<User>.Enter(User obj)
		{
			
		}

		void Samebest.Game.IStage<User>.Leave(User obj)
		{
			
		}

		void Samebest.Game.IStage<User>.Update(User obj)
		{
			
		}

		Samebest.Remoting.Value<bool> IVerify.CreateAccount(string name, string password)
		{
			throw new NotImplementedException();
		}

		Samebest.Remoting.Value<LoginResult> IVerify.Login(string name, string password)
		{
			throw new NotImplementedException();
		}

		void IVerify.Quit()
		{
			throw new NotImplementedException();
		}
	}
}
