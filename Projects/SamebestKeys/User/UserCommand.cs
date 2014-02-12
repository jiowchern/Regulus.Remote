using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Projects.SamebestKeys
{
    using Regulus.Extension;
    class UserCommand
    {
        private Utility.Console.IViewer _View;
        private Utility.Command _Command;
        System.Collections.Generic.Dictionary<object, string[]> _RemoveCommands;
        System.Collections.Generic.Dictionary<object, Action[]> _RemoveEvents;

        public UserCommand(Utility.Console.IViewer view, Utility.Command command)
        {            
            this._View = view;
            this._Command = command;
            _RemoveCommands = new Dictionary<object, string[]>();
            _RemoveEvents = new Dictionary<object, Action[]>();
        }

        internal void Register(Regulus.Project.SamebestKeys.IUser user)
        {
            user.ConnectProvider.Supply += ConnectProvider_Supply;
            user.ConnectProvider.Unsupply += _Unsupply;

            user.VerifyProvider.Supply += VerifyProvider_Supply;
            user.VerifyProvider.Unsupply += _Unsupply;

            user.ParkingProvider.Supply += ParkingProvider_Supply;
            user.ParkingProvider.Unsupply += _Unsupply;
        }

        void ParkingProvider_Supply(Project.SamebestKeys.IParking obj)
        {
            _Command.Register("Back", obj.Back);

            _RemoveCommands.Add(obj, new string[] 
            {
                "Back"  
            });
        }

        void VerifyProvider_Supply(Project.SamebestKeys.IVerify verify)
        {
            _Command.RemotingRegister<string, string, bool>("CreateAccount", verify.CreateAccount, (result) =>
            {
                if (result)
                {
                    _View.WriteLine("建立成功");
                }
                else
                    _View.WriteLine("建立失敗");
            });

            _Command.RemotingRegister<string, string, Regulus.Project.SamebestKeys.LoginResult>("Login", verify.Login, (result) =>
            {
                if (result == Project.SamebestKeys.LoginResult.Success)
                {
                    _View.WriteLine("登入成功");
                }
                else if (result == Project.SamebestKeys.LoginResult.Error)
                    _View.WriteLine("登入失敗");
                else if (result == Project.SamebestKeys.LoginResult.RepeatLogin)
                    _View.WriteLine("重複登入");
            });
            _Command.Register("Quit", verify.Quit);


            _RemoveCommands.Add(verify, new string[] 
            {
                "CreateAccount"  , "Quit" , "Login"
            });
        }
       

        void ConnectProvider_Supply(Project.SamebestKeys.IConnect connect)
        {
            _Command.RemotingRegister<string, int, bool>("Connect", connect.Connect, (result) => 
            {
                if (result)
                {
                    _View.WriteLine("連線成功");
                }
                else
                    _View.WriteLine("連線失敗");
            });

            _RemoveCommands.Add(connect, new string[] 
            {
                "Connect" 
            });
        }

        

        internal void Unregister(Regulus.Project.SamebestKeys.IUser user)
        {

            user.ConnectProvider.Supply -= ConnectProvider_Supply;
            user.ConnectProvider.Unsupply -= _Unsupply;

            user.VerifyProvider.Supply -= VerifyProvider_Supply;
            user.VerifyProvider.Unsupply -= _Unsupply;

            user.ParkingProvider.Supply -= ParkingProvider_Supply;
            user.ParkingProvider.Unsupply -= _Unsupply;

            foreach (var command in _RemoveCommands)
            {
                foreach (var cmd in command.Value)
                {
                    _Command.Unregister(cmd);
                }
            }

            foreach (var removerEvent in _RemoveEvents)
            {
                var removers = removerEvent.Value;
                foreach (var remover in removers)
                {
                    remover();
                }
            }
        }


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
