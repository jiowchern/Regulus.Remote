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
        IObservableActor[] _ActorInfomation;

        
        public Adventure(IObservableActor[] actor_infomation, Remoting.ISoulBinder binder, IMap zone)
        {
            _ActorInfomation = actor_infomation;
            _Map = zone;
            
            this._Binder = binder;

            
            
        }

        void Regulus.Game.IStage.Enter()
        {
            _Binder.Bind<IAdventure>(this);
            
            
            
        }

        void Regulus.Game.IStage.Leave()
        {
                        
            _Binder.Unbind<IAdventure>(this);            
        }


        

        void Regulus.Game.IStage.Update()
        {
            
        }
        
        
    }
}
