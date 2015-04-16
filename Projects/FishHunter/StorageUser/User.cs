using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Storage
{
    class User : IUser
    {
        private Regulus.Remoting.IAgent _Agent;
        Regulus.Utility.Updater _Updater;
        public User(Regulus.Remoting.IAgent agent)
        {
            
            this._Agent = agent;
            _Updater = new Regulus.Utility.Updater();

            _Remoting = new Regulus.Remoting.User(agent);
        }


        Regulus.Remoting.User _Remoting;
        Regulus.Remoting.User IUser.Remoting
        {
            get { return _Remoting; }
        }

        Regulus.Remoting.Ghost.IProviderNotice<IVerify> IUser.VerifyProvider
        {
            get { return _Agent.QueryProvider<IVerify>(); }
        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Update();
            return true;
        }

        void Regulus.Framework.ILaunched.Launch()
        {
            _Updater.Add(_Agent);
            _Updater.Add(_Remoting);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Updater.Shutdown();
        }


        Regulus.Remoting.Ghost.IProviderNotice<T> IUser.QueryProvider<T>()
        {
            return _Agent.QueryProvider<T>();
        }
    }
}
