
using System;
namespace Regulus.Project.TurnBasedRPG
{

    using Regulus.Project.TurnBasedRPG.Serializable;
    using Samebest.Remoting;

    public interface IVerify
    {
        
        Value<bool> CreateAccount(string name, string password);
        Value<LoginResult> Login(string name, string password);
        void Quit();        
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

	public interface IMapInfomation
	{
        Value<long> QueryTime();
	}

    public interface IPlayer
    {
        Guid Id { get; }
        void Ready();
        void Logout();
        void ExitWorld();        
        void SetPosition(float x,float y);		
        void SetVision(int vision);
        void Walk(float direction);
        void Stop();		
    }
    public interface IObservedAbility
    {
        Guid Id { get; }
        Regulus.Types.Vector2 Position { get; }        
        event Action<MoveInfomation> ShowActionEvent;
    }

	public interface IMoverAbility
	{
		void Update(long time, CollisionInformation collision_information);

        void Act(ActionStatue action_statue, float move_speed, float direction);
    }
    
}