using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game.Stage
{
    class Adventure : Regulus.Game.IStage, IAdventure
    {
        
        

        public delegate void OnParking();
        public event OnParking ParkingEvent;

        private Remoting.ISoulBinder _Binder;
        IMap _Map;
        ActorInfomation _ActorInfomation;

        Entity _Entity;
        public Adventure(ActorInfomation actor_infomation , Remoting.ISoulBinder binder, IMap zone)
        {
            _ActorInfomation = actor_infomation;
            _Map = zone;
            
            this._Binder = binder;

            _Entity = new Entity(_ActorInfomation.Id);
            
        }

        void Regulus.Game.IStage.Enter()
        {
            _Binder.Bind<IAdventure>(this);
            _Map.Enter(_Entity);            
            
            
        }

        void Regulus.Game.IStage.Leave()
        {
            
            _Map.Leave(_Entity);
            _Binder.Unbind<IAdventure>(this);            
        }


        

        void Regulus.Game.IStage.Update()
        {
            
        }
        
        
    }
}
