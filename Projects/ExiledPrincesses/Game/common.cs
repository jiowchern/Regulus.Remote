
using System;
namespace Regulus.Project.ExiledPrincesses
{
    
    using Regulus.Remoting;

    public interface IUserStatus
    {
        void Ready();
        event Action<UserStatus> StatusEvent;
    }

    public interface IVerify
    {        
        Value<bool> CreateAccount(string name, string password);
        Value<LoginResult> Login(string name, string password);
        Value<IVerify> Get();
        void Quit();        
    };

    public interface ITone
    {        
        
    };

    public interface IAdventure
    {
        
    }

    public interface IStorage
    {
        Value<AccountInfomation> FindAccountInfomation(string name);
		
        void Add(AccountInfomation ai);        
    }    

}
