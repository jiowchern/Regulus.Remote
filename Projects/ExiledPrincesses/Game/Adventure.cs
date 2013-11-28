using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    

    public interface ITeammate
    {
        int Dex { get; }
        Strategy Specializes { get; }
    }

    public class Monster : ITeammate
    {


        int ITeammate.Dex
        {
            get { throw new NotImplementedException(); }
        }

        Strategy ITeammate.Specializes
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class Teammate : ITeammate
    {
        private readonly ActorInfomation _Actor;

        public Teammate(ActorInfomation actor)
        {
            // TODO: Complete member initialization
            _Actor = actor;
        }


        int ITeammate.Dex
        {
            get { throw new NotImplementedException(); }
        }

        Strategy ITeammate.Specializes
        {
            get { throw new NotImplementedException(); }
        }
    }
    
    class Adventurer
    {
        public string Map;
        public Regulus.Project.ExiledPrincesses.Contingent.FormationType Formation;
        public ITeammate[] Teammates;
    }
}
namespace Regulus.Project.ExiledPrincesses.Game.Stage
{
    
    partial class Adventure : Regulus.Game.IStage
    {
        
        private Remoting.ISoulBinder _Binder;
        IZone _Zone;        
        Adventurer _Adventurer;
        public Adventure(Adventurer adventurer, Remoting.ISoulBinder binder, IZone zone)
        {
            _Adventurer = adventurer;
            _Zone = zone;            
            this._Binder = binder;
            
        }

        private void _ObtainMap(IMap map)
        {
            map.Initial(_Adventurer.Formation, _Adventurer.Teammates);
            map.ToMapEvent += (name) =>
            {
                map.Release();
                _Zone.Destory(map);
                _CreateMap(name);       
            };

            map.ToToneEvent += _ToTone;
            
        }

        public delegate void OnToTone(string name);
        public event OnToTone ToToneEvent;
        void _ToTone(string name)
        {
            ToToneEvent(name);
        }

        private void _CreateMap(string name)
        {
            _Zone.Create(name).OnValue += _ObtainMap;
        }
        

        void Regulus.Game.IStage.Enter()
        {
            _CreateMap(_Adventurer.Map);
        }

        void Regulus.Game.IStage.Leave()
        {
            
        }

        void Regulus.Game.IStage.Update()
        {
            
        }
        
        
    }

    
}
