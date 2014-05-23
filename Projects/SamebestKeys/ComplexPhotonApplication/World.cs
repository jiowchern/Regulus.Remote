using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

	interface IWorld
	{
        Regulus.Remoting.Value<IRealm> Query(string level_name);
        Regulus.Remoting.Value<IRealm> Find(Guid id);		
	}

	class World : IWorld , Regulus.Utility.IUpdatable
	{
        Dictionary<string, Realm> _Singletons;
        Regulus.Utility.TUpdater<Realm> _Realms;                
		private Remoting.Time _Time;

		public World(Remoting.Time time)
		{
			this._Time = time;
            _Realms = new Utility.TUpdater<Realm>();
            _Singletons = new Dictionary<string, Realm>();
		}


		void Regulus.Framework.ILaunched.Launch()
		{            

		}        

		bool Regulus.Utility.IUpdatable.Update()
		{
            _Realms.Update();
			return true;
		}

		void Regulus.Framework.ILaunched.Shutdown()
		{
            _Realms.Shutdown();
		}
        Remoting.Value<IRealm> IWorld.Find(Guid id)
        {
            return new Remoting.Value<IRealm>((from level in _Realms.Objects where _GetId(level) == id select level).SingleOrDefault());
        }

        Remoting.Value<IRealm> IWorld.Query(string realm_name)
        {
            var result = new Remoting.Value<IRealm>();
            var realm = _Query(realm_name);
            if (realm != null)
            {
                _Initial(result, realm);
            }
            else
                result.SetValue(null);
            return result;
        }

        

        private Realm _Query(string realm_name)
        {
            Data.Realm realm = GameData.Instance.FindLevel(realm_name);
            if (realm.Singleton)
            {
                var instance = _Find(realm.Name);
                if (instance != null)
                {
                    return instance;
                }
            }

            return _Create(realm , _Time);
        }

        private Realm _Create(Data.Realm realm, Remoting.Time time)
        {
            var instance = new Realm.Generator(realm).Create();
            
            if (realm.Singleton)
            {
                _Register(instance ,  realm.Name);
            }
            return instance;
        }

        private void _Register(Realm instance, string name)
        {
            _Singletons.Add(name, instance);
        }
        
        private Realm _Find(string level_name)
        {
            return (from singleton in _Singletons where singleton.Key == level_name select singleton.Value).SingleOrDefault();
        }


        private void _PushToUpdater(Realm map)
        {
            _Realms.Add(map);
        }

        private void _Initial(Remoting.Value<IRealm> result, Realm realm)
        {            
            _PushToUpdater(realm);           
        }
        

        
        
        private Guid _GetId(IRealm level)
        {
            return level.Id;
        }
    }
}
