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

	class World : IWorld , Regulus.Game.IFramework
	{
		List<Map>	_Maps ;
		Regulus.Game.FrameworkRoot	_Frameworks;
		private Remoting.Time _Time;

		public World(Remoting.Time time)
		{
			// TODO: Complete member initialization
			this._Time = time;

        
		}
		void Game.IFramework.Launch()
		{
			_Frameworks = new Game.FrameworkRoot();
			_Maps = new List<Map>();
			Map map = _BuildMap("Ferdinand");
			_Maps.Add(map);
			
			_Frameworks.AddFramework(map);
			map.SetTime(_Time);
		}

		private Map _BuildMap(string name)
		{
			var map = new Map(name);
			return map;
		}

		bool Game.IFramework.Update()
		{
			_Frameworks.Update();
			return true;
		}

		void Game.IFramework.Shutdown()
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
