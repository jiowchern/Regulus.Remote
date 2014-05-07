using Regulus.Project.SamebestKeys.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
	/// <summary>
	/// 登入驗證
	/// </summary>
    class Verify : IVerify
    {
        
        Regulus.Remoting.Value<bool> IVerify.CreateAccount(string name, string password)
        {

            if (_Stroage.FindAccountInfomation(name) == null)
            {
                AccountInfomation ai = new AccountInfomation();
                ai.Name = name;
                ai.Password = password;
                ai.Id = Guid.NewGuid();
                ai.LevelRecords = new string[] { "demo" };
                _Stroage.Add(ai);
                return true;
            }
            return false;
        }

        public event Action<Serializable.AccountInfomation> LoginSuccess;
        

        IStorage _Stroage;
        public Verify( IStorage stroage)
        {
            
            _Stroage = stroage;
            
        }
        Regulus.Remoting.Value<LoginResult> IVerify.Login(string name, string password)
        {
            var ai = _Stroage.FindAccountInfomation(name);
            if (ai != null && ai.Password == password)
            {
                LoginSuccess(ai);
                return LoginResult.Success;
            }
            return LoginResult.Error;
        }


        public event Action QuitEvent;
        void IVerify.Quit()
        {
            if (QuitEvent != null)
            {
                QuitEvent();
                QuitEvent = null;
            }
        }
    }
}
