using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    class BattlefieldResources : Regulus.Utility.Singleton<BattlefieldResources>
    {
        Dictionary<Guid, BattlefieldPrototype> _BattlefieldPrototypes;
        public BattlefieldResources()
        {
            _BattlefieldPrototypes = new Dictionary<Guid, BattlefieldPrototype>();
        }
        internal BattlefieldPrototype Find(Guid guid)
        {
            BattlefieldPrototype battlefieldPrototype;
            if (_BattlefieldPrototypes.TryGetValue(guid, out battlefieldPrototype))
                return battlefieldPrototype;
            return null;
        }
    }
}
