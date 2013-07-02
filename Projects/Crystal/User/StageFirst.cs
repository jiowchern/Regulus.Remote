using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal.Standalone.Stage
{
	public class First : Samebest.Game.IStage<Regulus.Project.Crystal.Standalone.Core>, Regulus.Project.Crystal.IVerify
	{
		Core _Core;
		void Samebest.Game.IStage<Core>.Enter(Core obj)
		{
			_Core = obj;
			_Core.Binder.Bind<Regulus.Project.Crystal.IVerify>(this);
		}

		void Samebest.Game.IStage<Core>.Leave(Core obj)
		{
			_Core.Binder.Unbind<Regulus.Project.Crystal.IVerify>(this);
		}

		void Samebest.Game.IStage<Core>.Update(Core obj)
		{
			
		}

		Samebest.Remoting.Value<bool> IVerify.CreateAccount(string name, string password)
		{
			var val =  _Core.Storage.FindAccountInfomation(name);

			Samebest.Remoting.Value<bool> ret = new Samebest.Remoting.Value<bool>();
			val.OnValue += (AccountInfomation result_account) =>
			{
				if (result_account == null)
				{
					AccountInfomation ai = new AccountInfomation();
					ai.Id = Guid.NewGuid();
					ai.Name = name;
					ai.Password = password;
					_Core.Storage.Add(ai);
					ret.SetValue(true);
				}
				ret.SetValue(false);
			};
			return ret;
		}

		

		Samebest.Remoting.Value<LoginResult> IVerify.Login(string name, string password)
		{
			return LoginResult.Success;
		}

		void IVerify.Quit()
		{
			
		}
	}
}
