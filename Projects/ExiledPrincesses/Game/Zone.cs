using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public interface IMap
    {
        void Enter(IEntity entity);
        void Leave(IEntity entity);

        Regulus.Remoting.Value<bool> BattleRequest(Guid id);

        event OnMapBattle BattleResponseEvent;
    }

    
    

    public class Zone 
    {
        Regulus.Project.ExiledPrincesses.Game.Hall _Hall;
        IStorage _Storage;
        Battle.Zone _Battle;
        Regulus.Game.FrameworkRoot _Loopers;
        Map _Map;
        public Zone(IStorage storage)
        {
            _Storage = storage;
            _Hall = new Hall();
            _Battle = new Battle.Zone();
            _Map = new Map(_Battle);

            _Loopers = new Regulus.Game.FrameworkRoot();
            _Loopers.AddFramework(_Battle);
        }

        public void Enter(Regulus.Remoting.ISoulBinder binder)
        {
            _Hall.CreateUser(binder, _Storage, _Map , _Battle);
        }

        public void Update()
        {
            _Hall.Update();
            _Loopers.Update();
        }

       

        
        



        
    }
}
