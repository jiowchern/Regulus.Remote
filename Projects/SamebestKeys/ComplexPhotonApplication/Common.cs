
using System;
namespace Regulus.Project.SamebestKeys
{

    using Regulus.Project.SamebestKeys.Serializable;
    using Regulus.Remoting;
    

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
        Value<Types.Polygon[]> QueryWalls();
	}

    public interface IPlayer
    {
        Guid Id { get; }
        string Name { get; }
        float Speed { get; }
        float Direction { get; }		
        
        void Ready();
        void Logout();
        void ExitWorld();        
        void SetPosition(float x,float y);		
        void SetVision(int vision);

        void SetSpeed(float speed);
        void Walk(float direction);
        void Stop(float direction);
        void Say(string message);
		
        void BodyMovements(ActionStatue action_statue);
        Value<string> QueryMap();

        void Goto(string map , float x , float y);
        
    }



    public interface IObservedAbility 
    {
        string Name { get; }
        Guid Id { get; }
        Regulus.Types.Vector2 Position { get; }
        float Direction { get; }        
        event Action<MoveInfomation> ShowActionEvent;
        event Action<string> SayEvent;
    }


    public interface IMoverAbility
    {
        Regulus.Types.Polygon Polygon { get; }

        void Act(ActionStatue action_statue, float move_speed, float direction);

        void Update(long time, System.Collections.Generic.IEnumerable<Regulus.Types.Polygon> obbs);
    }
    /*public interface IMoverAbility
    {
        Regulus.Utility.OBB Obb { get; }

        void Act(ActionStatue action_statue, float move_speed, float direction);

        void Update(long time, System.Collections.Generic.IEnumerable<Utility.OBB> obbs);
    }*/
    
}


