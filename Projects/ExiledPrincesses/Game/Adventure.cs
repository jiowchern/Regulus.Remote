using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses.Game.Stage
{
    
    partial class Adventure : Regulus.Game.IStage , IAdventure
    {
        
        private Remoting.ISoulBinder _Binder;
        IZone _Zone;
        ILevels _Levels;
        Adventurer _Adventurer;
        Squad _Squad;
        public Adventure(Adventurer adventurer, Remoting.ISoulBinder binder, IZone zone)
        {
            _Adventurer = adventurer;
            _Zone = zone;            
            this._Binder = binder;
            _Squad = new Squad(adventurer.Formation ,_Adventurer.Teammates, _Adventurer.Controller);
        }

        private void _ObtainMap(ILevels map)
        {
            if (map != null)
            {
                _Levels = map;
                map.ToLevelsEvent += _CreateLevels;
                map.ToTownEvent += _ToTown;
            }
            else
            {
                throw new SystemException("找不到地圖");
            }
        }

        public delegate void OnToTone(string name);
        public event OnToTone ToToneEvent;
        void _ToTown(string name)
        {
            ToToneEvent(name);
        }

        private void _CreateLevels(string name)
        {

            _Zone.Create(name, _Adventurer.Formation, _Squad).OnValue += _ObtainMap;
            _ChangeLevels(name);
        }
        

        void Regulus.Game.IStage.Enter()
        {
            
            
            _Binder.Bind<IAdventure>(this);
            _CreateLevels(_Adventurer.Map);
        }

        void Regulus.Game.IStage.Leave()
        {
            _Levels.Leave(_Squad);
            _Binder.Unbind<IAdventure>(this);
        }

        void Regulus.Game.IStage.Update()
        {
            
        }



        Remoting.Value<string> IAdventure.QueryLevels()
        {
            return _Adventurer.Map;
        }
        event Action<string> _ChangeLevels;
        event Action<string> IAdventure.ChangeLevels
        {
            add { _ChangeLevels += value; }
            remove { _ChangeLevels -= value; }
        }
    }

    
}
