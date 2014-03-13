using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    public class GameDataBuilder
    {
        IGameDataSetter _Setter;
        public GameDataBuilder(IGameDataSetter setter)
        {
            _Setter = setter;
        }
        public void Build(byte[] stream)
        {
            _Setter.Set(Regulus.Utility.IO.Serialization.Read<Data.Map[]>(stream));
        }
    }

    public interface IGameDataSetter
    {
        void Set(Data.Map[] map);
    }

    public class GameData : Regulus.Utility.Singleton<GameData>, IGameDataSetter
    {
        List<Data.Map> _Maps;

        public GameData()
        {            
            _Maps = new List<Data.Map>();
        }

        void _Add(Data.Map[] maps)
        {
            _Maps.AddRange(maps);
        }
        void _Set(Data.Map[] maps)
        {                     
            _Maps = maps.ToList();
        }

        internal Data.Map FindMap(string name)
        {
            var map = (from m in _Maps where m.Name == name select m).FirstOrDefault();
            return map;
        }



        void IGameDataSetter.Set(Data.Map[] map)
        {
            _Add(map);
        }

        public IEnumerable<Data.Map> Maps 
        {
            get { return _Maps;  }
        }
    }
}
