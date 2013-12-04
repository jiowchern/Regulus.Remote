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

            _System.TownProvider.Supply += _OnTownSupply; 
            _System.TownProvider.Unsupply += _Unsupply;

            _System.AdventureProvider.Supply += _OnAdventureSupply;
            _System.AdventureProvider.Unsupply += _Unsupply;

            _System.AdventureIdleProvider.Supply += _OnAdventureIdleSupply;
            _System.AdventureIdleProvider.Unsupply += _Unsupply;

            _System.AdventureGoProvider.Supply += _OnAdventureGoSupply;
            _System.AdventureGoProvider.Unsupply += _Unsupply;


            _System.AdventureChoiceProvider.Supply += _OnAdventureChoiceSupply;
            _System.AdventureChoiceProvider.Unsupply += _Unsupply;

            _System.ActorProvider.Supply += _OnActorSupply;
            _System.ActorProvider.Unsupply += _Unsupply;
            

            
        }

        
        internal void Release()
        {
            _System.ActorProvider.Supply -= _OnActorSupply;
            _System.ActorProvider.Unsupply -= _Unsupply;

            _System.AdventureChoiceProvider.Supply -= _OnAdventureChoiceSupply;
            _System.AdventureChoiceProvider.Unsupply -= _Unsupply;

            _System.AdventureGoProvider.Supply -= _OnAdventureGoSupply;
            _System.AdventureGoProvider.Unsupply -= _Unsupply;

            _System.AdventureIdleProvider.Supply -= _OnAdventureIdleSupply;
            _System.AdventureIdleProvider.Unsupply -= _Unsupply;

            _System.VerifyProvider.Supply -= _OnVerifySupply;
            _System.VerifyProvider.Unsupply -= _Unsupply;

            _System.StatusProvider.Supply -= _OnStatusSupply;
            _System.StatusProvider.Unsupply -= _Unsupply;

            _System.TownProvider.Supply -= _OnTownSupply;
            _System.TownProvider.Unsupply -= _Unsupply;

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

        private void _OnActorSupply(IActor obj)
        {
            _View.WriteLine("Actor id:" + obj.Pretotype );
        }

        private void _OnAdventureChoiceSupply(IAdventureChoice obj)
        {
            _View.Write("Map [");
            foreach(var map in obj.Maps)
            {
                _View.Write(" " + map+" ");
            }
            _View.WriteLine("]");

            _View.Write("Town [");
            foreach (var town in obj.Town)
            {
                _View.Write(" " + town + " ");
            }
            _View.WriteLine("]");

            _Command.Register<string>("ChoiceMap", obj.GoMap);
            _Command.Register<string>("ChoiceTown", obj.GoTown);

            _RemoveCommands.Add(obj, new string[] 
            {
                "ChoiceMap" , "ChoiceTown"
            });
        }

        private void _OnAdventureGoSupply(IAdventureGo obj)
        {
            obj.ForwardEvent += _OnForward;
            _RemoveEvents.Add(obj, new Action[] 
            {
                ()=>{obj.ForwardEvent -= _OnForward;}
            });
        }

        private void _OnForward(long time_tick, float position, float speed)
        {
            _View.WriteLine("移動 時間:" + time_tick + " 位置:" + position + "速度:" + speed);
        }


        

        void _OnEnableChipMessage(string obj)
        {
            _View.WriteLine(obj);
        }
  
        
        private void _OnAdventureSupply(IAdventure adventure)
        {
            _Command.RemotingRegister<string>("QueryLevels", adventure.QueryLevels, (levels) => 
            {
                _View.WriteLine("目前關卡 : " + levels);
            });

            adventure.ChangeLevels += _OnChangeLevels;
            _RemoveEvents.Add(adventure, new Action[] 
            {
                ()=>{adventure.ChangeLevels -= _OnChangeLevels;}
            });
            
            _RemoveCommands.Add(adventure, new string[] 
            {
                "QueryLevels"
            });
        }

        private void _OnChangeLevels(string levels)
        {
            _View.WriteLine("切換關卡 : " + levels);
        }

        private void _OnTownSupply(ITown town)
        {
            _View.WriteLine("抵達城鎮:"+ town.Name);
            _View.Write("前往區域 [");
            foreach (var map in town.Maps)
            {
                _View.Write(" "+map+" ");
            }
            _View.WriteLine("]");

            _Command.Register<string>("Goto", town.ToMap);
            _RemoveCommands.Add(town, new string[] 
            {
                "Goto"   
            });
        }

        

        private void _OnStatusSupply(IUserStatus status)
        {
            
            status.StatusEvent += _OnUserStatusChanged;
            _RemoveEvents.Add(status , new Action[] 
            {
                ()=>
                {                    
                    status.StatusEvent -= _OnUserStatusChanged;
                }
            });

            _Command.Register("Ready", status.Ready );
            _Command.RemotingRegister<long>("Time", status.QueryTime, (time) =>
            {
                _View.WriteLine("現在時間 : " + System.TimeSpan.FromTicks(time) + " ( "+ time +" )"  );
            });
            _RemoveCommands.Add(status, new string[] { "Ready" , "Time" 
            });
        }

        private void _OnChangeTown(string obj)
        {
            _View.WriteLine("切換城鎮 : " + obj);
        }

        void _OnUserStatusChanged(UserStatus status)
        {
            _View.WriteLine("遊戲狀態改變 : " + status);
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

        private void _OnAdventureIdleSupply(IAdventureIdle obj)
        {
            _Command.Register("go", obj.GoForwar);
            _RemoveCommands.Add(obj, new string[] 
            {
                "go" 
            });
        }        
    }
}

