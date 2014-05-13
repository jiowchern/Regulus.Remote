using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

	interface IWorld
	{
        Regulus.Remoting.Value<IMap> Create(string level_name);
        Regulus.Remoting.Value<IMap> Find(Guid id);		
	}

	class World : IWorld , Regulus.Utility.IUpdatable
	{
        Regulus.Utility.TUpdater<Level> _Levels;
        Regulus.Utility.TUpdater<Map> _Updater;
        
		private Remoting.Time _Time;

		public World(Remoting.Time time)
		{			
			this._Time = time;
            _Updater = new Regulus.Utility.TUpdater<Map>();
            _Levels = new Utility.TUpdater<Level>();
		}
		void Regulus.Framework.ILaunched.Launch()
		{            
		}        

		bool Regulus.Utility.IUpdatable.Update()
		{
			_Updater.Update();
			return true;
		}

		void Regulus.Framework.ILaunched.Shutdown()
		{
			_Updater.Shutdown();
		}
        Remoting.Value<IMap> IWorld.Find(Guid id)
        {
            return new Remoting.Value<IMap>((from map in _Updater.Objects where _GetId(map) == id select map).SingleOrDefault());
        }

        Remoting.Value<IMap> IWorld.Create(string map_name)
        {
            var map = _Create(map_name);
            if (map != null)
            {
                _Initial(map);
                return new Remoting.Value<IMap>(map);
            }
            return new Remoting.Value<IMap>(null);
        }

        private Map _Create(string map_name)
        {
            return new Map(map_name, _Time);
        }


        private void _AddUpdater(Map map)
        {
            _Updater.Add(map);
        }
        private void _Initial(Map map)
        {                        
            _AddUpdater(map);
            _RegisterRemoveMap(map, () => { _RemoveUpdater(map); });
        }

        private void _RemoveUpdater(Map map)
        {
            _Updater.Remove(map);
        }
        private void _RegisterRemoveMap(IMap map,Action remover_handler)
        {
            map.ShutdownEvent += remover_handler;
        }
        
        private Guid _GetId(IMap map)
        {
            return map.Id;
        }
    }
}
