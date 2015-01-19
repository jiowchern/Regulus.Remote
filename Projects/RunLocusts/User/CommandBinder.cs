using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imdgame.RunLocusts
{
    using Regulus.Extension;
    class CommandBinder
    {
        Regulus.Utility.Console.IViewer _View;
        Regulus.Utility.Command _Command;
        public CommandBinder(Regulus.Utility.Console.IViewer view , Regulus.Utility.Command command)
        {
            _View = view;
            _Command = command;
            _RemoveCommands = new Dictionary<object, string[]>();
            _RemoveEvents = new Dictionary<object, Action[]>();
        }
        internal void LookUser(IUser user)
        {
            user.ConnectProvider.Supply += ConnectProvider_Supply;
            user.ConnectProvider.Unsupply += _Unsupply;

            user.OnlineProvider.Supply += OnlineProvider_Supply;
            user.OnlineProvider.Unsupply += _Unsupply;
        }

        void OnlineProvider_Supply(Regulus.Utility.IOnline obj)
        {
            _Command.Register("Disconnect", obj.Disconnect );
            obj.DisconnectEvent += obj_DisconnectEvent;

            _Command.Register("Ping", () => { _View.WriteLine("Ping is " + obj.Ping.ToString()); });

            _RemoveCommands.Add(obj, new string[] 
            {
                "Ping"   , "Disconnect"
            });

            _RemoveEvents.Add(obj, new Action[] 
            {
                ()=>{obj.DisconnectEvent -= obj_DisconnectEvent;}
            });

        }

        void obj_DisconnectEvent()
        {
            _View.WriteLine("disconnect.");
        }

        void ConnectProvider_Supply(Regulus.Utility.IConnect obj)
        {

            _Command.RemotingRegister<string,int,bool>("Connect" , obj.Connect , _ConnectResult );

            _RemoveCommands.Add(obj, new string[] 
            {
                "Connect"  
            });
        }

        private void _ConnectResult(bool result)
        {
            if (result)
                _View.WriteLine("connect success.");
            else
                _View.WriteLine("connect fail.");
        }

        internal void UnlookUser(IUser user)
        {
            user.ConnectProvider.Supply -= ConnectProvider_Supply;
            user.ConnectProvider.Unsupply -= _Unsupply;
            user.OnlineProvider.Supply -= OnlineProvider_Supply;
            user.OnlineProvider.Unsupply -= _Unsupply;

            foreach (var command in _RemoveCommands)
            {
                foreach (var cmd in command.Value)
                {
                    _Command.Unregister(cmd);
                }
            }
            _RemoveCommands.Clear();

            foreach (var removerEvent in _RemoveEvents)
            {
                var removers = removerEvent.Value;
                foreach (var remover in removers)
                {
                    remover();
                }
            }
            _RemoveEvents.Clear();
        }


        System.Collections.Generic.Dictionary<object, string[]> _RemoveCommands;
        System.Collections.Generic.Dictionary<object, Action[]> _RemoveEvents;

        void _Unsupply<T>(T obj)
        {

            string[] commands;
            if (_RemoveCommands.TryGetValue(obj, out commands))
            {
                foreach (var command in commands)
                {
                    _Command.Unregister(command);
                }
            }

            Action[] removers;
            if (_RemoveEvents.TryGetValue(obj, out removers))
            {
                foreach (var remover in removers)
                {
                    remover();
                }
            }

            _RemoveCommands.Remove(obj);
            _RemoveEvents.Remove(obj);
        }
    }
}
