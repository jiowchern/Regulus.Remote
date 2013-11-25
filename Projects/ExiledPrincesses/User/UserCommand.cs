using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses
{
    using Regulus.Extension;
    public class UserCommand
    {
        private IUser _System;
        Regulus.Utility.Console.IViewer _View;
        Regulus.Utility.Command _Command;
        System.Collections.Generic.Dictionary<object, Action[]> _RemoveEvents;
        
        public UserCommand(IUser system , Regulus.Utility.Console.IViewer view , Regulus.Utility.Command command)
        {
            _RemoveEvents = new Dictionary<object, Action[]>();
            _System = system;
            _View = view;
            _Command = command;

            _System.VerifyProvider.Supply += _OnVerifySupply ;            
            _System.VerifyProvider.Unsupply += _Unsupply;

            _System.StatusProvider.Supply += _OnStatusSupply;
            _System.StatusProvider.Unsupply += _Unsupply;

            _System.ParkingProvider.Supply += _OnParkingSupply; 
            _System.ParkingProvider.Unsupply += _Unsupply;

            _System.AdventureProvider.Supply += _OnAdventureSupply;
            _System.AdventureProvider.Unsupply += _Unsupply;

            
        }
        internal void Release()
        {
            

            _System.VerifyProvider.Supply -= _OnVerifySupply;
            _System.VerifyProvider.Unsupply -= _Unsupply;

            _System.StatusProvider.Supply -= _OnStatusSupply;
            _System.StatusProvider.Unsupply -= _Unsupply;

            _System.ParkingProvider.Supply -= _OnParkingSupply;
            _System.ParkingProvider.Unsupply -= _Unsupply;

            _System.AdventureProvider.Supply -= _OnAdventureSupply;
            _System.AdventureProvider.Unsupply -= _Unsupply;

            
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


        

        void _OnEnableChipMessage(string obj)
        {
            _View.WriteLine(obj);
        }


        

        

        
        
        private void _OnAdventureSupply(IAdventure adventure)
        {
            
            _RemoveCommands.Add(adventure, new string[] 
            {
                
            });
        }

        private void _OnParkingSupply(IParking parking)
        {
            
            
            

            

            _RemoveCommands.Add(parking, new string[] 
            {
                
            });
        }

        

        private void _OnStatusSupply(IUserStatus status)
        {            
            
            status.StatusEvent += _OnUserStatusChanged;
            _RemoveEvents.Add(status , new Action[] 
            {
                ()=>{status.StatusEvent -= _OnUserStatusChanged;}
            });

            _Command.Register("Ready", status.Ready );
            _RemoveCommands.Add(status, new string[] { "Ready" });
        }

        void _OnUserStatusChanged(UserStatus status)
        {
            _View.WriteLine("遊戲狀態改變" + status);
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

        System.Collections.Generic.Dictionary<object, string[]> _RemoveCommands = new Dictionary<object, string[]>();
        private void _OnVerifySupply(IVerify obj)
        {
            _Command.RemotingRegister<string,string,bool>("CreateAccount", obj.CreateAccount, (result) => 
            {

            });
            _Command.RemotingRegister<string, string, LoginResult>("Login", obj.Login, (result) => 
            {
                if (result == LoginResult.Success)
                    _View.WriteLine("登入成功.");
                else
                    _View.WriteLine("登入失敗.");
            });
            _Command.Register("Exit" , obj.Quit);

            _RemoveCommands.Add(obj, new string[] 
            {
                "CreateAccount" , "Login" , "Exit"
            });
        }

        
    }
}

