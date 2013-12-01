using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses.Game
{
    public class LocalTime : Regulus.Utility.Singleton<Regulus.Remoting.Time>
    {
    }

    public interface IZone
    {
        Regulus.Remoting.Value<IMap> Create(string map, Contingent.FormationType formationType, ITeammate[] teammate);
        void Destory(Guid map);
    }    
    
    public class Zone : IZone
    {
        Regulus.Project.ExiledPrincesses.Game.Hall _Hall;
        IStorage _Storage;

        Regulus.Utility.Updater<Map> _Loopers;
        
        public Zone(IStorage storage)
        {
            
            _Storage = storage;
            _Hall = new Hall();
            _Loopers = new Regulus.Utility.Updater<Map>();
            _Maps = new Dictionary<Guid, Map>();
        }

        public void Enter(Regulus.Remoting.ISoulBinder binder)
        {
            _Hall.CreateUser(binder, _Storage, this);
        }

        public void Update()
        {
            LocalTime.Instance.Update();
            _Hall.Update();
            _Loopers.Update();
        }

        Dictionary<Guid, Map> _Maps;        

        void IZone.Destory(Guid id)
        {
            Map map;
            if (_Maps.TryGetValue(id, out map))
            {
                map.Release();
                _Loopers.Remove(map);
                _Maps.Remove(id);
            }
            
        }
        
        Remoting.Value<IMap> IZone.Create(string name, Contingent.FormationType formation_type, ITeammate[] teammate)
        {
            var mapPrototype  = MapResources.Instance.Find(name);
            if (mapPrototype != null)
            {
                var map = new Map(mapPrototype);
                map.Initialize(formation_type, teammate);
                _Maps.Add(map.Id, map);
                _Loopers.Add(map);
                return map;    
            }
            return new Remoting.Value<IMap>(null);
        }
    }
}
