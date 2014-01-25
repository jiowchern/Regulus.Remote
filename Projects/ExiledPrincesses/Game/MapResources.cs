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
            _Add(_Maps, "Teaching", "Credits", new Station() { Position = 400, Kind = Station.KindType.Combat, Id = 1 } ,
                new Station() { Position = 700, Kind = Station.KindType.Choice, Id = 1 }
                );            
            _Add(_Maps, "Test1", "Credits",
                new Station() { Position = 200, Kind = Station.KindType.Combat, Id = 2 },
                new Station() { Position = 600, Kind = Station.KindType.Combat, Id = 3 },
                new Station() { Position = 700, Kind = Station.KindType.Choice, Id = 2 }
                );
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
