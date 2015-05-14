using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework.Stage
{
    class OnBoard<TUser>: Regulus.Utility.IStage
        where TUser : class, Regulus.Utility.IUpdatable
    {
        public delegate void OnDone();
        public event OnDone DoneEvent;
        private UserProvider<TUser> _UserProvider;
        private Utility.Command _Command;
        Regulus.Utility.CenterOfUpdateable _Updater;
        
        
        public OnBoard(UserProvider<TUser> user_provider, Utility.Command command)
        {
            _Updater = new Utility.CenterOfUpdateable();
            this._UserProvider = user_provider;
            this._Command = command;
        }

        
        void Regulus.Utility.IStage.Enter()
        {

            _Updater.Add(_UserProvider);
            _Command.Register<string>("SpawnUser[UserName]" , _Spawn);
            _Command.Register<string>("UnpawnUser[UserName]", _Unspawn);
            _Command.Register<string>("SelectUser[UserName]", _Select);
        }

        void Regulus.Utility.IStage.Leave()
        {
            _Command.Unregister("SelectUser");
            _Command.Unregister("SpawnUser");
            _Command.Unregister("UnpawnUser");
            _Updater.Shutdown();
        }

        void _Spawn(string name)
        {
            _UserProvider.Spawn(name);
        }

        void _Unspawn(string name)
        {            
            _UserProvider.Unspawn(name);
        }

        void _Select(string name)
        {
            _UserProvider.Select(name);            
        }

        void Regulus.Utility.IStage.Update()
        {
            _Updater.Working();
        }
    }
}
