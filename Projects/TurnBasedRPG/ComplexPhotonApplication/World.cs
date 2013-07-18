using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{

	interface IWorld
	{		
		void Into(string map_name, Entity entity);
		void Left(string map_name, Entity entity);
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

		void IWorld.Into(string map_name, Entity entity)
		{
			var map = (from m in _Maps where m.Name == map_name select m).FirstOrDefault();
			if(map != null)
			{
				map.Into(entity);
			}
		}

		void IWorld.Left(string map_name , Entity entity)
		{
			var map = (from m in _Maps where m.Name == map_name select m).FirstOrDefault();
			if(map != null)
			{
				map.Left(entity);
			}
		}
	}
}
