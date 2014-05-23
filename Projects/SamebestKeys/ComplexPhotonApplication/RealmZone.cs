using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    
    partial class Realm
    {
        interface IZone
        {


            IMap Find(string map_name);

            IMap EnterMap { get;  }
        }
        class Zone : Regulus.Utility.IUpdatable , IZone
	    {
            Map[] _Maps;
            Regulus.Utility.Updater _Updater;            

            public Zone(Map[] maps)
            {
                _Maps = maps;            
                _Updater = new Utility.Updater();
            }

            bool Utility.IUpdatable.Update()
            {                
                _Updater.Update();
                return true;
            }

            void Framework.ILaunched.Launch()
            {
                foreach (var map in _Maps)
                {
                    _Updater.Add(map);
                }
            }

            void Framework.ILaunched.Shutdown()
            {
                _Updater.Shutdown();
            }

            IMap IZone.Find(string map_name)
            {
                return (from map in _Maps where map.Name == map_name select map).SingleOrDefault();
            }

            IMap IZone.EnterMap
            {
                get
                {
                    throw new NotImplementedException();
                }                
            }
        }
    }
    
}
