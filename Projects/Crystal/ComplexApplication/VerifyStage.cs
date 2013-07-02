using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal
{
    class VerifyStage : Samebest.Game.IStage<User>, IVerify
	{
        IStorage _Storage;
        User _User;
        public VerifyStage(User user, IStorage storage)
        {
            _Storage = storage;
            _User = user;
        }
		
		void Samebest.Game.IStage<User>.Enter(User obj)
		{
            obj.Provider.Bind<IVerify>(this);
		}

		void Samebest.Game.IStage<User>.Leave(User obj)
		{
            obj.Provider.Unbind<IVerify>(this);
		}

		void Samebest.Game.IStage<User>.Update(User obj)
		{
            
		}

        Samebest.Remoting.Value<bool> IVerify.CreateAccount(string name, string password)
        {
            Samebest.Remoting.Value<bool> ret = new Samebest.Remoting.Value<bool>();
            var val = _Storage.FindAccountInfomation(name);
            val.OnValue += (account_infomation) =>
            {
                if (account_infomation != null)
                {
                    var ai = new AccountInfomation();
                    ai.Name = name;
                    ai.Password = password;
                    ai.Id = Guid.NewGuid();
                    _Storage.Add(ai);

                    ret.SetValue(true);
                }
                ret.SetValue(false);
            };

            return ret;
        }

        

        Samebest.Remoting.Value<LoginResult> IVerify.Login(string name, string password)
        {
            Samebest.Remoting.Value<LoginResult> ret = new Samebest.Remoting.Value<LoginResult>();
            var val = _Storage.FindAccountInfomation(name);
            val.OnValue += (account_infomation) =>
            {
                if (account_infomation != null && account_infomation.Password == password)
                {
                    ret.SetValue(LoginResult.Success);
                }
                ret.SetValue(LoginResult.Fail);
            };
            return ret;
        }

        void IVerify.Quit()
        {
            _User.Quit();            
        }
    }
}
