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
        Regulus.Remoting.Value<ILevels> Create(string map, Contingent.FormationType formationType, Squad squad);        
    }    
    
    public class Zone : IZone
    {
        Regulus.Project.ExiledPrincesses.Game.Hall _Hall;
        IStorage _Storage;

        Regulus.Utility.Updater<Levels> _Loopers;
        
        public Zone(IStorage storage)
        {
            
            _Storage = storage;
            _Hall = new Hall();
            _Loopers = new Regulus.Utility.Updater<Levels>();
            
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

        

        
        
        Remoting.Value<ILevels> IZone.Create(string name, Contingent.FormationType formation_type, Squad squad)
        {
            var mapPrototype  = MapResources.Instance.Find(name);
            if (mapPrototype != null)
            {
                var levels = new Levels(mapPrototype, squad);                
                levels.ReleaseEvent += () => 
                {                    
                    _Loopers.Remove(levels);
                };                
                _Loopers.Add(levels);
                return levels;    
            }
            return new Remoting.Value<ILevels>(null);
        }
    }
}
