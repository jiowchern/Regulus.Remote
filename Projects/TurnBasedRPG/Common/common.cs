
using System;
namespace Regulus.Project.TurnBasedRPG
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
        void Ready();
        void Logout();
        void ExitWorld();
        Value<int> SetData(int i);
        Value<int> GetData();
        
        void SetPosition(float x,float y);
        void SetVision(int vision);
    }
    public interface IObservedAbility
    {
        Guid Id { get; }
        Regulus.Types.Vector2 Position { get; }
    }
    
}