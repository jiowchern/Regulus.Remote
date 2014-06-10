using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    partial class Realm
    {        
        public class Generator
        {
            Data.Realm _Data;

            public Generator(Data.Realm data)
            {
                _Data = data;
            }

            internal Realm Create()
            {
                return _Create(_Data);
            }

            private Realm _Create(Data.Realm data)
            {
                Realm.Zone zone = _BuildStage(data);
                Realm.JoinCondition joinCondition = _BuildJoinCondition(data);
                return new Realm(new Team(zone,joinCondition ), zone, LocalTime.Instance);
            }

            private Realm.Zone _BuildStage(Data.Realm data)
            {
                var stages = data.Stages;
                var mapDatas = from stage in stages
                            join map in GameData.Instance.Maps
                            on stage.MapName equals map.Name
                            select map;

                Map[] maps = _BuildMap(mapDatas);

                return new Realm.Zone(maps);
            }

            private Map[] _BuildMap(IEnumerable<Data.Map> datas)
            {
                List<Map> maps = new List<Map>();
                foreach(var data in datas)
                {
                    maps.Add(new Map(data.Name, LocalTime.Instance));                    
                }
                return maps.ToArray();
            }

            private Realm.JoinCondition _BuildJoinCondition(Data.Realm data)
            {
                if (data.NotLimit())
                {
                    return new Realm.FreeJoinCondition();
                }
                else
                    return new Realm.LimitJoinCondition(data.NumberForPlayer);
            }
        }
    }
    
}
