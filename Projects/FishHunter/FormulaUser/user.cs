using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VGame.Project.FishHunter.Formula
{
    class User : IUser
    {
        Regulus.Utility.CenterOfUpdateable _Updater;
        Regulus.Remoting.User _User;

        private Regulus.Remoting.IAgent _Agent;

        public User(Regulus.Remoting.IAgent agent)
        {
            this._Agent = agent;
            _Updater = new Regulus.Utility.CenterOfUpdateable();
            _User = new Regulus.Remoting.User(_Agent);

        }

        bool Regulus.Utility.IUpdatable.Update()
        {
            _Updater.Working();
            return true;
        }

        void Regulus.Framework.IBootable.Launch()
        {
            _Updater.Add(_User);
        }

        void Regulus.Framework.IBootable.Shutdown()
        {
            _Updater.Shutdown();
        }

        Regulus.Remoting.User IUser.Remoting
        {
            get { return _User; }
        }


        Regulus.Remoting.Ghost.INotifier<IVerify> IUser.VerifyProvider
        {
            get { return _Agent.QueryNotifier<IVerify>(); }
        }


        Regulus.Remoting.Ghost.INotifier<IFishStageQueryer> IUser.FishStageQueryerProvider
        {
            get { return _Agent.QueryNotifier<IFishStageQueryer>(); }
        }
        
    }
}
