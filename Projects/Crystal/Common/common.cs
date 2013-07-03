
using System;
namespace Regulus.Project.Crystal
{
    using Regulus.Remoting;
    public interface IVerify
    {        
        Value<bool> CreateAccount(string name, string password);
        Value<LoginResult> Login(string name, string password);
        void Quit();        
    };


    public interface IStorage
    {
        Value<AccountInfomation> FindAccountInfomation(string name);

        void Add(AccountInfomation ai);
    }

}
