﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
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

        void Regulus.Framework.ILaunched.Launch()
        {
            _Updater.Add(_User);
        }

        void Regulus.Framework.ILaunched.Shutdown()
        {
            _Updater.Shutdown();
        }

        Regulus.Remoting.User IUser.Remoting
        {
            get { return _User; }
        }


        Regulus.Remoting.Ghost.IProviderNotice<IVerify> IUser.VerifyProvider
        {
            get { return _Agent.QueryProvider<IVerify>(); }
        }





        Regulus.Remoting.Ghost.IProviderNotice<IPlayer> IUser.PlayerProvider
        {
            get { return _Agent.QueryProvider<IPlayer>(); }
        }
    }
}
