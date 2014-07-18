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


        static Data.Skill[] _Skills = { new Data.Skill() { Id = 1, Begin = 0.26f, Effective = 0.26f, End = 0.26f, Energy = 10 , Capture = false , CaptureBounds = new Types.Rect() , UseMode = ActorMode.All} ,
                                      new Data.Skill() { Id = 2, Begin = 0.33f, Effective = 0.03f, End = 0.69f, Energy = 10, Capture = true, CaptureBounds = new Types.Rect(-1,-1,2,2 ) , UseMode = ActorMode.Alert , Param1 = 10}};


        public Data.Scene[] Scenes { get { return _Realms ; } }

        static Data.Scene[] _Realms = 
        {
            /*new Data.Scene()
            {
                Front = "SL_1A",
                Name = "SL_1C",
                Singleton = false,
                Stages = new [] { new Data.Stage { MapName = "SL_1C" }  }
            },*/
            new Data.Scene()
            {
                Front = "",
                Name = "SC_1A",
                Singleton = true,
                Stages = new [] { new Data.Stage { MapName = "SC_1A" }  }
            },

            new Data.Scene()
            {
                Front = "",
                Name = "SL_1A",
                Singleton = false,
                Stages = new [] { new Data.Stage { MapName = "SL_1A" }  }
            },
            new Data.Scene()
            {
                Front = "",
                Name = "SL_1B",
                Singleton = false,
                Stages = new [] { new Data.Stage { MapName = "SL_1B" }  }
            },
            new Data.Scene()
            {
                Front = "",
                Name = "SL_1B_2",
                Singleton = false,
                Stages = new [] { new Data.Stage { MapName = "SL_1B_2" }  }
            },
            new Data.Scene()
            {
                Front = "",
                Name = "SL_1C",
                Singleton = false,
                Stages = new [] { new Data.Stage { MapName = "SL_1C" }  }
            },
            new Data.Scene()
            {
                Front = "",
                Name = "SL_1K",
                Singleton = false,
                Stages = new [] { new Data.Stage { MapName = "SL_1K" }  }
            },
            
            
            new Data.Scene()
            {
                Front = "",
                Name = "Test",
                Singleton = true,
                Stages = new [] { new Data.Stage { MapName = "Test" } , new Data.Stage { MapName = "Ark" } }
            },
            new Data.Scene()
            {
                Front = "",
                Name = "Room",
                Singleton = false,
                //Stages = new [] { new Data.Stage { MapName = "Test" } , new Data.Stage { MapName = "Ark" } }
                Stages = new [] {new Data.Stage { MapName = "Room" }  }
            }
        };
        internal Data.Skill FindSkill(int id)
        {
            return (from s in _Skills where s.Id == id select s).SingleOrDefault();
        }

        internal Data.Scene FindRealm(string realm_name)
        {
            return (from realm in _Realms where realm.Name == realm_name select realm).SingleOrDefault();
        }
    }
}
