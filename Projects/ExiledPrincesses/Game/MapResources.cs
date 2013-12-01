using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    class MapResources : Regulus.Utility.Singleton<MapResources>
    {
        Dictionary<string, MapPrototype> _Maps;
        public MapResources()
        {
            _Maps = new Dictionary<string, MapPrototype>();            
        }
        internal MapPrototype Find(string name)
        {
            MapPrototype mapPrototype;
            if (_Maps.TryGetValue(name, out mapPrototype))
            {
                return mapPrototype;
            }
            return null;
        }
    }


}
