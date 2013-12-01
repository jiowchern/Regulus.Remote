using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
            if (map != null)
            {
                map.ToMapEvent += (name) =>
                {
                    _Zone.Destory(map.Id);
                    _CreateMap(name);
                };

                map.ToToneEvent += _ToTone;
            }
            else
            {
                throw new SystemException("找不到地圖");
            }
        }

        public delegate void OnToTone(string name);
        public event OnToTone ToToneEvent;
        void _ToTone(string name)
        {
            ToToneEvent(name);
        }

        private void _CreateMap(string name)
        {
            _Zone.Create(name, _Adventurer.Formation, _Adventurer.Teammates ).OnValue += _ObtainMap;
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
