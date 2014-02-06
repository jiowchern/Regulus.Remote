using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{

	interface IWorld
	{
        Regulus.Remoting.Value<IMap> Find(string map_name);		
	}

	class World : IWorld , Regulus.Utility.IUpdatable
	{
		List<Map>	_Maps ;

		Regulus.Utility.Updater<Regulus.Utility.IUpdatable> _Frameworks;
		private Remoting.Time _Time;

		public World(Remoting.Time time)
		{
			// TODO: Complete member initialization
			this._Time = time;

        
		}
		void Regulus.Framework.ILaunched.Launch()
		{
			_Frameworks = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();
			_Maps = new List<Map>();

			
            _AddMap(_BuildMap("Ferdinand"));

            
            _AddMap(_BuildMap("Chasel"));            
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
