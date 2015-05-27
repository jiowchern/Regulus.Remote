using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Storage
{
    class User : IUser
    {
        private Regulus.Remoting.IAgent _Agent;
        Regulus.Utility.CenterOfUpdateable _Updater;
        public User(Regulus.Remoting.IAgent agent)
        {
            
            this._Agent = agent;
            _Updater = new Regulus.Utility.CenterOfUpdateable();
            _Remoting = new Regulus.Remoting.User(agent);
        }


        Regulus.Remoting.User _Remoting;
        Regulus.Remoting.User IUser.Remoting
        {
            get { return _Remoting; }
        }

        Regulus.Remoting.Ghost.INotifier<IVerify> IUser.VerifyProvider
        {
            get { return _Agent.QueryNotifier<IVerify>(); }
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Updater.Add(_Agent);
            _Updater.Add(_Remoting);
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Updater.Shutdown();
        }


        Regulus.Remoting.Ghost.INotifier<T> IUser.QueryProvider<T>()
        {
            return _Agent.QueryNotifier<T>();
        }


        Regulus.Remoting.Ghost.INotifier<IStorageCompetnces> IUser.StorageCompetncesProvider
        {
            get { return _Agent.QueryNotifier<IStorageCompetnces>(); }
        }
    }
}
