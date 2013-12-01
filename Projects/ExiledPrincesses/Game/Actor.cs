using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    class ActorResource : Regulus.Utility.Singleton<ActorResource>
    {
        Dictionary<int, ActorPrototype> _Actors;
        public ActorResource()
        {
            _Actors = new Dictionary<int, ActorPrototype>();
            _Add(1, Strategy.Sword, Strategy.Sword
                    ,new Ability(100, 100, 100, 100 , new int[] { 5 })
                    , new Ability(100, 100, 100, 200, new int[] { 5 })
                    , new Ability(100, 100, 100, 300, new int[] { 5 }));
            _Add(2, Strategy.Sword, Strategy.Staff
                    , new Ability(100, 100, 100, 100, new int[] { 6 })
                    , new Ability(100, 100, 100, 200, new int[] { 6 })
                    , new Ability(100, 100, 100, 300, new int[] { 6 }));
            _Add(3, Strategy.Sword, Strategy.Shield
                    , new Ability(100, 100, 100, 100, new int[] { 7 })
                    , new Ability(100, 100, 100, 200, new int[] { 7 })
                    , new Ability(100, 100, 100, 300, new int[] { 7 }));
            _Add(4, Strategy.Sword, Strategy.Ax
                    , new Ability(100, 100, 100, 100, new int[] { 8 })
                    , new Ability(100, 100, 100, 200, new int[] { 8 })
                    , new Ability(100, 100, 100, 300, new int[] { 8 }));
            
        }

        private void _Add(int id, Strategy strategy1, Strategy strategy2,params Ability[] abilitys)
        {
            _Actors.Add(id, new ActorPrototype() { Specializes = strategy1, Weakness = strategy2, Abilitys = abilitys });
        }
        internal ActorPrototype Find(int id)
        {
            return _Actors[id]; 
        }
        
    }
}
