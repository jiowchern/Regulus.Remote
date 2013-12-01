
using System;
namespace Regulus.Project.ExiledPrincesses
{
    
    using Regulus.Remoting;

    public interface IUserStatus
    {
        void Ready();
        event Action<UserStatus> StatusEvent;
        Value<long> QueryTime();
    }

    public interface IVerify
    {        
        Value<bool> CreateAccount(string name, string password);
        Value<LoginResult> Login(string name, string password);        
        void Quit();        
    };

    public interface ITown
    {
        string[] Maps { get; }
        void ToMap(string map);
    };

    public interface IAdventure
    {
        
    }

    public interface IStorage
    {
        Value<AccountInfomation> FindAccountInfomation(string name);
		
        void Add(AccountInfomation ai);        
    }

    public interface IAdventureIdle
    {
        void GoForwar();
    }

    
    public interface IAdventureGo
    {
         event Action<long /*time_tick*/ , float /*position*/ , float /*speed*/> ForwardEvent;
    }

    public interface IAdventureChoice
    {
        string[] Maps {get;}
        string[] Town { get; }

        void GoMap(string map);
        void GoTown(string tone);
    }
}
