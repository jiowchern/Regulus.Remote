
using System;
namespace Regulus.Project.Crystal
{
    using Samebest.Remoting;
    public interface IVerify
    {        
        Value<bool> CreateAccount(string name, string password);
        Value<LoginResult> Login(string name, string password);
        void Quit();        
    };
}
