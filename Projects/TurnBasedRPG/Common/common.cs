
using System;
namespace Regulus.Project.TurnBasedRPG.Common
{

    using Regulus.Project.TurnBasedRPG.Serializable;
    using Samebest.Remoting;    

    public interface IVerify
    {
        Value<bool> CreateAccount(string name, string password);
        Value<bool> Login(string name, string password );
        void Quit();
        event System.Action RepeatLogin;
    };

    public interface IParking
    {
        Value<bool> CheckActorName(string name );
        Value<bool> CreateActor(EntityLookInfomation cai);
        Value<EntityLookInfomation[]> DestroyActor(string name);
        Value<EntityLookInfomation[]> QueryActors();
        void Back();
        Value<bool> Select(string name);
    }

    public interface IPlayer
    {        
        void Logout();
        void ExitWorld();
        Value<int> SetData(int i);
        Value<int> GetData();
        event Action<EntityInfomation>  IntoEvent;
        event Action<Guid>              LeftEvent;

        
    }

    
}