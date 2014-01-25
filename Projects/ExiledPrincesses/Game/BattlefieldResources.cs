using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    class BattlefieldResources : Regulus.Utility.Singleton<BattlefieldResources>
    {
        Dictionary<int, BattlefieldPrototype> _BattlefieldPrototypes;
        public BattlefieldResources()
        {
            _BattlefieldPrototypes = new Dictionary<int, BattlefieldPrototype>();
            _BattlefieldPrototypes.Add(1 , new BattlefieldPrototype() { Formation = Contingent.FormationType.Auxiliary , Money = 1000 , Enemys = new int[] {1,1}});
            _BattlefieldPrototypes.Add(2, new BattlefieldPrototype() { Formation = Contingent.FormationType.Auxiliary, Money = 1000, Enemys = new int[] { 1 } });
            _BattlefieldPrototypes.Add(3, new BattlefieldPrototype() { Formation = Contingent.FormationType.Auxiliary, Money = 1000, Enemys = new int[] { 1 ,1,1} });

            _BattlefieldPrototypes.Add(4, new BattlefieldPrototype() { Formation = Contingent.FormationType.Auxiliary, Money = 1000, Enemys = new int[] { 1, 1 } });
            _BattlefieldPrototypes.Add(5, new BattlefieldPrototype() { Formation = Contingent.FormationType.Auxiliary, Money = 1000, Enemys = new int[] { 1 } });
            _BattlefieldPrototypes.Add(6, new BattlefieldPrototype() { Formation = Contingent.FormationType.Auxiliary, Money = 1000, Enemys = new int[] { 1, 1, 1 } });
        }
        internal BattlefieldPrototype Find(int id)
        {
            BattlefieldPrototype battlefieldPrototype;
            if (_BattlefieldPrototypes.TryGetValue(id, out battlefieldPrototype))
                return battlefieldPrototype;
            return null;
        }
    }
}
