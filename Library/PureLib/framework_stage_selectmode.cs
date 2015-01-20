using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework.Stage
{
    class SelectMode<TUser> : Regulus.Utility.IStage
        where TUser : class , Regulus.Utility.IUpdatable
    {
        public delegate void OnDone<TUser>(Regulus.Framework.UserProvider<TUser> console) where TUser : class , Regulus.Utility.IUpdatable;
        public event OnDone<TUser> DoneEvent;

        Regulus.Utility.Command _Command;
        Regulus.Framework.GameModeSelector<TUser> _Selector;

        public event Action InitialedEvent;

        public SelectMode(Regulus.Framework.GameModeSelector<TUser> mode_selector , Regulus.Utility.Command command )
        {
            _Command = command;
            _Selector = mode_selector;
            
        }

        private void _ObtainConsole(Regulus.Framework.UserProvider<TUser> console)
        {
            DoneEvent(console);
        }

        void Regulus.Utility.IStage.Enter()
        {
            _Selector.GameConsoleEvent += _ObtainConsole;
            _Command.Register<string>("CreateMode" , _CreateGameConsole );

            InitialedEvent();
        }
        
        void Regulus.Utility.IStage.Leave()
        {

            _Command.Unregister("CreateMode");
            _Selector.GameConsoleEvent -= _ObtainConsole;
        }

        void Regulus.Utility.IStage.Update()
        {            
        }

        void _CreateGameConsole(string name)
        {
            _Selector.CreateUserProvider(name);
        }
    }
}
