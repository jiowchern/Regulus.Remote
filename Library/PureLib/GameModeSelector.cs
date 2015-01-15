using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{
    public class GameModeSelector<TUser>
        where TUser : class, Regulus.Utility.IUpdatable
    {
        struct Provider
        {
            public string Name;
            public Regulus.Framework.IUserFactoty<TUser> Factory;
        }

        System.Collections.Generic.List<Provider> _Providers;
        
        private Regulus.Utility.Console.IViewer _View;

        public delegate void OnGameConsole(UserProvider<TUser> console);
        public event OnGameConsole GameConsoleEvent;

        Regulus.Utility.Command _Command;
        public GameModeSelector(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view )
        {
            _Command = command;
            this._View = view;
            
            _Providers = new List<Provider>();
        }
        public void AddFactoty(string name, Regulus.Framework.IUserFactoty<TUser> user_factory)
        {
            _Providers.Add(new Provider() { Name = name, Factory = user_factory });
            _View.WriteLine( string.Format("Added {0} factory." , name));
        }
        
        public UserProvider<TUser> CreateGameConsole(string name)
        {
            Regulus.Framework.IUserFactoty<TUser> factory = _Find(name);
            if (factory == null)
            {
                _View.WriteLine(string.Format("Game mode {0} not found.", name));
                return null;
            }
            
            _View.WriteLine(string.Format("Create game console factory : {0}.", name));

            var console = new UserProvider<TUser>(factory, _View, _Command);
            if (GameConsoleEvent != null)
                GameConsoleEvent(console);
            return console;
        }

        private IUserFactoty<TUser> _Find(string name)
        {
            return (from provider in _Providers where provider.Name == name select provider.Factory).SingleOrDefault();
        }

        
    }
}
