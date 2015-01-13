using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{
    public class GameModeSelector<TUser>
        where TUser : class
    {
        struct Provider
        {
            public string Name;
            public Regulus.Framework.IUserFactoty<TUser> Factory;
        }

        System.Collections.Generic.List<Provider> _Providers;
        private Regulus.Utility.Command _Command;
        private Regulus.Utility.Console.IViewer _View;
        Framework.ICommandParsable<TUser> _Parser;
        public GameModeSelector(Regulus.Utility.Command command, Regulus.Utility.Console.IViewer view , Framework.ICommandParsable<TUser> parser)
        {            
            this._Command = command;
            this._View = view;
            _Parser = parser;
            _Providers = new List<Provider>();

            command.Register<string, GameConsole<TUser>>("CreateGameConsole", CreateGameConsole, ( dummy ) => { });
        }

        ~GameModeSelector()
        {
            _Command.Unregister("CreateGameConsole");            
        }

        public void AddFactoty(string name, Regulus.Framework.IUserFactoty<TUser> user_factory)
        {
            _Providers.Add(new Provider() { Name = name, Factory = user_factory });
            _View.WriteLine( string.Format("Added {0} factory." , name));
        }
        
        public GameConsole<TUser> CreateGameConsole(string name )
        {
            Regulus.Framework.IUserFactoty<TUser> factory = _Find(name);
            _View.WriteLine(string.Format("Create Game Console Factory:{0}.", name));
            return new GameConsole<TUser>(_Parser, factory, _View, _Command);
        }

        private IUserFactoty<TUser> _Find(string name)
        {
            return (from provider in _Providers where provider.Name == name select provider.Factory).SingleOrDefault();
        }
    }
}
