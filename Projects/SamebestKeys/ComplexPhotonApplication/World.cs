using Regulus.Project.SamebestKeys.Dungeons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

	interface IWorld
	{
        Regulus.Remoting.Value<Regulus.Project.SamebestKeys.Dungeons.IScene> Query(string level_name);
        Regulus.Remoting.Value<Regulus.Project.SamebestKeys.Dungeons.IScene> Find(Guid id);		
	}

	class World : IWorld , Regulus.Utility.IUpdatable
	{
        Dictionary<string, Regulus.Project.SamebestKeys.Dungeons.Scene> _Singletons;
        Regulus.Utility.TUpdater<Regulus.Project.SamebestKeys.Dungeons.Scene> _Realms;                
		private Remoting.Time _Time;

		public World(Remoting.Time time)
		{
			this._Time = time;
            _Realms = new Utility.TUpdater<Scene>();
            _Singletons = new Dictionary<string, Scene>();
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
        Remoting.Value<IScene> IWorld.Find(Guid id)
        {
            return new Remoting.Value<IScene>((from level in _Realms.Objects where _GetId(level) == id select level).SingleOrDefault());
        }

        Remoting.Value<IScene> IWorld.Query(string realm_name)
        {
            var result = new Remoting.Value<IScene>();
            var realm = _Query(realm_name);
            if (realm != null)
            {
                _Initial(result, realm);                
            }
                            
            return realm;
        }

        

        private Scene _Query(string realm_name)
        {
            Data.Scene realm = GameData.Instance.FindRealm(realm_name);
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

        private Scene _Create(Data.Scene realm, Remoting.Time time)
        {
            var instance = new Generator(realm).Create();
            
            if (realm.Singleton)
            {
                _Register(instance ,  realm.Name);
            }
            return instance;
        }

        private void _Register(Scene instance, string name)
        {
            _Singletons.Add(name, instance);
        }
        
        private Scene _Find(string level_name)
        {
            return (from singleton in _Singletons where singleton.Key == level_name select singleton.Value).SingleOrDefault();
        }


        private void _PushToUpdater(Scene map)
        {
            _Realms.Add(map);
        }

        private void _Initial(Remoting.Value<IScene> result, Scene realm)
        {            
            _PushToUpdater(realm);           
        }
        

        
        
        private Guid _GetId(IScene level)
        {
            return level.Id;
        }
    }
}
