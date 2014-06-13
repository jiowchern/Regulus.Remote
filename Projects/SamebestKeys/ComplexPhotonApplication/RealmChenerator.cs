using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Dungeons
{
    public class Generator
    {
        Data.Scene _Data;
    
        public Generator(Data.Scene data)
        {
            _Data = data;
        }
    
        internal Scene Create()
        {
            return _Create(_Data);
        }
    
        private Scene _Create(Data.Scene data)
        {
            Zone zone = _BuildStage(data);
            JoinCondition joinCondition = _BuildJoinCondition(data);
            return new Scene(new Team(zone,joinCondition ), zone, LocalTime.Instance);
        }
    
        private Zone _BuildStage(Data.Scene data)
        {
            var stages = data.Stages;
            var mapDatas = from stage in stages
                        join map in GameData.Instance.Maps
                        on stage.MapName equals map.Name
                        select map;
    
            Map[] maps = _BuildMap(mapDatas);
    
            return new Zone(maps);
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
    
        private JoinCondition _BuildJoinCondition(Data.Scene data)
        {
            if (data.NotLimit())
            {
                return new FreeJoinCondition();
            }
            else
                return new LimitJoinCondition(data.NumberForPlayer);
        }
    }
}
