
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

    public interface IParking
    {        
        Value<ActorInfomation> SelectActor(string name);
    };

    public interface IAdventure
    {
        Value<bool> InBattle();
    }

    public interface IStorage
    {
        Value<AccountInfomation> FindAccountInfomation(string name);
		
        void Add(AccountInfomation ai);
        void Add(Pet pet);
        
        Value<Pet> FindPet(Guid id);
    }

    public interface IEntity
    {
        Guid Id { get; }
        T QueryAttrib<T>();
    }

    public interface IBattleAdmissionTickets
    {
        Value<IBattlerBehavior> Visit(Pet pet);
    }
    
    public interface IBattler
    {
        event Action<string> PassiveEvent;
        event Action<string> ActiveEvent;

        Value<Regulus.Project.ExiledPrincesses.Battle.Chip[]> QueryStabdby();
        Value<Regulus.Project.ExiledPrincesses.Battle.Chip[]> QueryEnable();
        Value<Pet> QueryPet();        
    }

    public interface IBattlerBehavior
    {
        Value<IBattler> QueryBattler();
        event Action<IReadyCaptureEnergy> SpawnReadyCaptureEnergyEvent;
        event Action<ICaptureEnergy> SpawnCaptureEnergyEvent;
        event Action<IEnableChip> SpawnEnableChipEvent;
        event Action<IDrawChip> SpawnDrawChipEvent;

        
        event Action    UnspawnReadyCaptureEnergyEvent;
        event Action    UnspawnCaptureEnergyEvent;
        event Action    UnspawnEnableChipEvent;
        event Action    UnspawnDrawChipEvent;
    }

    
    public interface IReadyCaptureEnergy
    {
        void UseChip(int[] chip_indexs);
        event Action<Regulus.Project.ExiledPrincesses.Battle.Chip[]> UsedChipEvent;
    }
    
    public interface ICaptureEnergy
    {        
        Value<EnergyGroup[]> Capture(int idx);
    }

    public interface IRemoveEnergy
    { 

    }
    public interface IEnableChip    
    {
        Value<BattleSpeed[]> QuerySpeeds();
        Value<bool> Enable(int index);
        void Done();
        
    }
    
    public interface IDrawChip
    {

        void Draw(int index);
    }
    

    

}
