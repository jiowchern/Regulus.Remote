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
        Regulus.Remoting.Value<IRealm> Create(string level_name);
        Regulus.Remoting.Value<IRealm> Find(Guid id);		
	}

	class World : IWorld , Regulus.Utility.IUpdatable
	{
        Regulus.Utility.TUpdater<Realm> _Levels;        
        
		private Remoting.Time _Time;

		public World(Remoting.Time time)
		{
			this._Time = time;
            _Levels = new Utility.TUpdater<Realm>();
            _Singletons = new Dictionary<string, Realm>();
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
        Remoting.Value<IRealm> IWorld.Find(Guid id)
        {
            return new Remoting.Value<IRealm>((from level in _Levels.Objects where _GetId(level) == id select level).SingleOrDefault());
        }

        Remoting.Value<IRealm> IWorld.Create(string level_name)
        {
            var level = _Create(level_name);
            if (level != null)
            {
                _Initial(level);
                return new Remoting.Value<IRealm>(level);
            }
            return new Remoting.Value<IRealm>(null);
        }

        private Realm _Create(string level_name)
        {
            Data.Realm level = GameData.Instance.FindLevel(level_name);
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

        private Realm _Create(Data.Realm level, Remoting.Time time)
        {
            var instance = new Realm(time);
            instance.Build(level);
            if (level.Singleton)
            {
                _Register(instance ,  level.Name);
            }
            return instance;
        }


        private void _Register(Realm instance, string name)
        {
            _Singletons.Add(name, instance);
        }


        
        Dictionary<string, Realm> _Singletons;
        private Realm _Find(string level_name)
        {
            return (from singleton in _Singletons where singleton.Key == level_name select singleton.Value).SingleOrDefault();
        }


        private void _AddUpdater(Realm map)
        {
            _Levels.Add(map);
        }
        private void _Initial(Realm map)
        {                        
            _AddUpdater(map);
            _RegisterRemoveMap(map, () => { _RemoveUpdater(map); });
        }

        private void _RemoveUpdater(Realm level)
        {
            _Levels.Remove(level);
        }
        private void _RegisterRemoveMap(IRealm map,Action remover_handler)
        {
            map.ShutdownEvent += remover_handler;
        }
        
        private Guid _GetId(IRealm level)
        {
            return level.Id;
        }
    }
}
