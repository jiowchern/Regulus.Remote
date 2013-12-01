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
            _Add(_Maps, "Teaching", "Credits", new Station() { Position = 100, Kind = Station.KindType.Choice, Id = 1 });
            _Add(_Maps, "Test1", "Credits", new Station() { Position = 100, Kind = Station.KindType.Choice, Id = 2 });
        }
        void _Add(Dictionary<string, MapPrototype> maps, string name , string tone, params Station[] stations)
        {
            maps.Add(name, new MapPrototype() { Tone = tone, Stations = stations });
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
