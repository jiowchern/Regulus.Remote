using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

	interface IWorld
	{
        Regulus.Remoting.Value<IMap> Find(string map_name);		
	}

	class World : IWorld , Regulus.Utility.IUpdatable
	{
		List<Map>	_Maps ;

		Regulus.Utility.Updater _Frameworks;
		private Remoting.Time _Time;

		public World(Remoting.Time time)
		{
			// TODO: Complete member initialization
			this._Time = time;

        
		}
		void Regulus.Framework.ILaunched.Launch()
		{
			_Frameworks = new Regulus.Utility.Updater();
			_Maps = new List<Map>();

            foreach(Data.Map map in GameData.Instance.Maps)
            {
                _AddMap(_BuildMap(map.Name));
            }            
		}        

        private void _AddMap(Map map)
        {
            _Maps.Add(map);
            _Frameworks.Add(map);
            map.SetTime(_Time);
        }

		private Map _BuildMap(string name)
		{
			var map = new Map(name);
			return map;
		}

		bool Regulus.Utility.IUpdatable.Update()
		{
			_Frameworks.Update();
			return true;
		}

		void Regulus.Framework.ILaunched.Shutdown()
		{
			_Frameworks.Shutdown();
		}



        Remoting.Value<IMap> IWorld.Find(string map_name)
        {
            var map = (from m in _Maps where m.Name == map_name select m).FirstOrDefault();
            if (map != null)
            {
                return map;
            }
            return new Remoting.Value<IMap>(null);
        }
    }
}
