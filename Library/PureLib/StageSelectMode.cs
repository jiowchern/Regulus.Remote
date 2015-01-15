using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework.Stage
{
    class SelectMode<TUser> : Regulus.Game.IStage
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

        void Regulus.Game.IStage.Enter()
        {
            _Selector.GameConsoleEvent += _ObtainConsole;
            _Command.Register<string>("CreateMode" , _CreateGameConsole );

            InitialedEvent();
        }
        
        void Regulus.Game.IStage.Leave()
        {

            _Command.Unregister("CreateMode");
            _Selector.GameConsoleEvent -= _ObtainConsole;
        }

        void Regulus.Game.IStage.Update()
        {            
        }

        void _CreateGameConsole(string name)
        {
            _Selector.CreateGameConsole(name);
        }
    }
}
