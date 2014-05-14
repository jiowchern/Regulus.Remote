using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    struct AAA
    {
        public int a;
    }
    struct AAAA
    {
        public AAA AAA;
        public int A;
        public int B;
        public int C;
        public int D;
    }

    class CCCC
    {
       public int A;
       public int B;
       public int C;
       public int D;
    }


	interface IWorld
	{
        Regulus.Remoting.Value<ILevel> Create(string level_name);
        Regulus.Remoting.Value<ILevel> Find(Guid id);		
	}

	class World : IWorld , Regulus.Utility.IUpdatable
	{
        Regulus.Utility.TUpdater<Level> _Levels;        
        
		private Remoting.Time _Time;

		public World(Remoting.Time time)
		{
			this._Time = time;
            _Levels = new Utility.TUpdater<Level>();
            _Singletons = new Dictionary<string, Level>();
		}


		void Regulus.Framework.ILaunched.Launch()
		{            
		}        

		bool Regulus.Utility.IUpdatable.Update()
		{
            _Levels.Update();
			return true;
		}

		void Regulus.Framework.ILaunched.Shutdown()
		{
            _Levels.Shutdown();
		}
        Remoting.Value<ILevel> IWorld.Find(Guid id)
        {
            return new Remoting.Value<ILevel>((from level in _Levels.Objects where _GetId(level) == id select level).SingleOrDefault());
        }

        Remoting.Value<ILevel> IWorld.Create(string level_name)
        {
            var level = _Create(level_name);
            if (level != null)
            {
                _Initial(level);
                return new Remoting.Value<ILevel>(level);
            }
            return new Remoting.Value<ILevel>(null);
        }

        private Level _Create(string level_name)
        {
            Data.Level level = GameData.Instance.FindLevel(level_name);
            if (level.Singleton)
            {
                var instance = _Find(level.Name);
                if (instance != null)
                {
                    return instance;
                }
            }

            return _Create(level , _Time);
        }

        private Level _Create(Data.Level level, Remoting.Time time)
        {
            var instance = new Level(time);
            instance.Build(level);
            if (level.Singleton)
            {
                _Register(instance ,  level.Name);
            }
            return instance;
        }


        private void _Register(Level instance, string name)
        {
            _Singletons.Add(name, instance);
        }


        
        Dictionary<string, Level> _Singletons;
        private Level _Find(string level_name)
        {
            return (from singleton in _Singletons where singleton.Key == level_name select singleton.Value).SingleOrDefault();
        }


        private void _AddUpdater(Level map)
        {
            _Levels.Add(map);
        }
        private void _Initial(Level map)
        {                        
            _AddUpdater(map);
            _RegisterRemoveMap(map, () => { _RemoveUpdater(map); });
        }

        private void _RemoveUpdater(Level level)
        {
            _Levels.Remove(level);
        }
        private void _RegisterRemoveMap(ILevel map,Action remover_handler)
        {
            map.ShutdownEvent += remover_handler;
        }
        
        private Guid _GetId(ILevel level)
        {
            return level.Id;
        }
    }
}
