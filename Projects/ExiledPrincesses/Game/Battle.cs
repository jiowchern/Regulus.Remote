using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Battle
{
    class Battler
    {
        public Guid Id { get; set; }

        public BattlerSide Side { get; set; }

        public Pet Pet { get; set; }
    }


    [Serializable]
    public class Chip
    {
        public Chip()
        {
            Passives = new int[0];
            Initiatives = new int[0];
            Red = new int[3];
            Yellow = new int[3];
            Green = new int[3];
            Power = new int[3];
        }
        public string Name;  
        public int[] Red;
        public int[] Yellow;
        public int[] Green;
        public int[] Power;
        public int[] Passives;
        public int[] Initiatives;
    };

    public interface IZone
    {
        Regulus.Remoting.Value<IBattleAdmissionTickets> Open(BattleRequester requester);        
    }
    
    

    
}

namespace Regulus.Project.ExiledPrincesses.Game.Stage
{
    class Battle : Regulus.Game.IStage
    {

        public delegate void OnEnd();
        public event OnEnd EndEvent;
        private ActorInfomation _ActorInfomation;
        private Remoting.ISoulBinder _Binder;
        
        IStorage _Storage;

        Pet _Pet;
        IBattleAdmissionTickets _BattleAdmissionTickets;
        public Battle(IBattleAdmissionTickets battle_admission_tickets, ActorInfomation actor_infomation, Remoting.ISoulBinder binder, IStorage stroage)
        {
            _BattleAdmissionTickets = battle_admission_tickets;
            _ActorInfomation = actor_infomation;
            _Binder = binder;
        
            _Storage = stroage;
        }
        void Regulus.Game.IStage.Enter()
        {
           
            var petResult = _Storage.FindPet(_ActorInfomation.Id);            
            petResult.OnValue += _OnPetReady;
            
        }

        
        void _OnPetReady(Pet pet)
        {
            _Pet = pet;
            
            var value = _BattleAdmissionTickets.Visit(_Pet);
            value.OnValue += _OnStart;
        }
        IBattler _Battler;
        void _OnStart(IBattlerBehavior battle_stage)
        {
            var val = battle_stage.QueryBattler();
            val.OnValue += (battler) =>
            {
                _Binder.Bind<IBattler>(battler);
                _Battler = battler;
            };

            battle_stage.SpawnReadyCaptureEnergyEvent += (obj) =>
            {
                _Binder.Bind<IReadyCaptureEnergy>(obj);
                
                battle_stage.UnspawnReadyCaptureEnergyEvent+= () =>
                {
                    _Binder.Unbind<IReadyCaptureEnergy>(obj);
                };
            };
            battle_stage.SpawnCaptureEnergyEvent += (obj) =>
            {
                _Binder.Bind<ICaptureEnergy>(obj);
                
                battle_stage.UnspawnCaptureEnergyEvent += () =>
                {
                    _Binder.Unbind<ICaptureEnergy>(obj);
                };
            };
            battle_stage.SpawnDrawChipEvent += (obj) =>
            {
                _Binder.Bind<IDrawChip>(obj);
                
                battle_stage.UnspawnDrawChipEvent+= () =>
                {
                    _Binder.Unbind<IDrawChip>(obj);
                };
            };
            battle_stage.SpawnEnableChipEvent += (obj) =>
            {
                _Binder.Bind<IEnableChip>(obj);
                
                battle_stage.UnspawnEnableChipEvent+= () =>
                {
                    _Binder.Unbind<IEnableChip>(obj);
                };
            };
        }
        

        void Regulus.Game.IStage.Leave()
        {
            _Binder.Unbind<IBattler>(_Battler);
        }

        void Regulus.Game.IStage.Update()
        {
            
        }


       
    }
}
