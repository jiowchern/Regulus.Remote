using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework.Stage
{
    class OnBoard<TUser>: Regulus.Game.IStage
        where TUser : class
    {
        public delegate void OnDone();
        public event OnDone DoneEvent;
        private UserProvider<TUser> _UserProvider;
        private Utility.Command _Command;

        
        public OnBoard()
        {
            
        }

        public OnBoard(UserProvider<TUser> user_provider, Utility.Command command)
        {
            
            this._UserProvider = user_provider;
            this._Command = command;
        }

        
        void Regulus.Game.IStage.Enter()
        {

            System.GC.Collect();
            _Command.Register<string>("SpawnUser" , _Spawn);
            _Command.Register<string>("UnpawnUser", _Unspawn);
            _Command.Register<string>("SelectUser", _Select);
        }

        void Regulus.Game.IStage.Leave()
        {
            _Command.Unregister("SelectUser");
            _Command.Unregister("SpawnUser");
            _Command.Unregister("UnpawnUser");
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

        void Regulus.Game.IStage.Update()
        {
            
        }
    }
}
