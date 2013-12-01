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
