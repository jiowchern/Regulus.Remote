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

            _System.BattleProvider.Supply += _OnBattleSupply;
            _System.BattleProvider.Unsupply += _Unsupply;

            _System.BattleReadyCaptureEnergyProvider.Supply += _OnBattleReadyCaptureEnergySupply;
            _System.BattleReadyCaptureEnergyProvider.Unsupply += _Unsupply;

            _System.BattleCaptureEnergyProvider.Supply += _OnBattleCaptureEnergyProviderSupply;
            _System.BattleCaptureEnergyProvider.Unsupply += _Unsupply;

            _System.BattleDrawChipProvider.Supply += _OnBattleDrawChipSupply;
            _System.BattleDrawChipProvider.Unsupply += _Unsupply;

            _System.BattleEnableChipProvider.Supply += _OnBattleEnableChipSupply;
            _System.BattleEnableChipProvider.Unsupply += _Unsupply;
        }
        internal void Release()
        {
            _System.BattleEnableChipProvider.Supply -= _OnBattleEnableChipSupply;
            _System.BattleEnableChipProvider.Unsupply -= _Unsupply;

            _System.VerifyProvider.Supply -= _OnVerifySupply;
            _System.VerifyProvider.Unsupply -= _Unsupply;

            _System.StatusProvider.Supply -= _OnStatusSupply;
            _System.StatusProvider.Unsupply -= _Unsupply;

            _System.ParkingProvider.Supply -= _OnParkingSupply;
            _System.ParkingProvider.Unsupply -= _Unsupply;

            _System.AdventureProvider.Supply -= _OnAdventureSupply;
            _System.AdventureProvider.Unsupply -= _Unsupply;

            _System.BattleProvider.Supply -= _OnBattleSupply;
            _System.BattleProvider.Unsupply -= _Unsupply;

            _System.BattleReadyCaptureEnergyProvider.Supply -= _OnBattleReadyCaptureEnergySupply;
            _System.BattleReadyCaptureEnergyProvider.Unsupply -= _Unsupply;

            _System.BattleCaptureEnergyProvider.Supply -= _OnBattleCaptureEnergyProviderSupply;
            _System.BattleCaptureEnergyProvider.Unsupply -= _Unsupply;

            _System.BattleDrawChipProvider.Supply -= _OnBattleDrawChipSupply;
            _System.BattleDrawChipProvider.Unsupply -= _Unsupply;

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


        private void _OnBattleEnableChipSupply(IEnableChip obj)
        {
            _Command.RemotingRegister<BattleSpeed[]>("QuerySpeed", obj.QuerySpeeds, (battle_speeds) =>
                {
                    _View.Write("順序");
                    foreach (var battleSpeed in battle_speeds)
                    {
                        _View.Write(battleSpeed.Name + "[" + battleSpeed.Speed + "] ,");                        
                    }

                    _View.WriteLine("");
                });
            _Command.RemotingRegister<int, bool>("CardLaunched", obj.Enable, (result)=> 
            {
                if (result==false)                
                    _View.WriteLine("能源不足卡片啟用失敗");
            });
            
            _Command.Register("Done", obj.Done);

            
            _RemoveCommands.Add(obj, new string[] 
            {
                "CardLaunched" , "Done" , "QuerySpeed"
            });

        }

        void _OnEnableChipMessage(string obj)
        {
            _View.WriteLine(obj);
        }


        private void _OnBattleSupply(IBattler obj)
        {
            
            _Command.RemotingRegister<Regulus.Project.ExiledPrincesses.Battle.Chip[]>("HandArea", obj.QueryStabdby, (chips) =>
            {
                _View.Write("待用區 ");
                foreach (var chip in chips)
                {

                    if (chip != null)
                    {
                        _View.Write("[ " + chip.Name + " ]");
                    }
                    else
                    {
                        _View.Write("[ 空 ]");
                    }
                }
                _View.WriteLine("");
            });

            _Command.RemotingRegister<Regulus.Project.ExiledPrincesses.Battle.Chip[]>("EnableZone", obj.QueryEnable, (chips) =>
               {
                   _View.Write("啟用區 ");
                   foreach (var chip in chips)
                   {

                       if (chip != null)
                       {
                           _View.Write("[ " + chip.Name + " ]");
                       }
                       else
                       {
                           _View.Write("[ 空 ]");
                       }
                   }
                   _View.WriteLine("");
               });
            _Command.RemotingRegister<Pet>("QueryPet", obj.QueryPet, 
                (pet) => 
                {
                    _View.WriteLine("取得寵物[" + pet.Name + "]");                    
                });
            _RemoveCommands.Add(obj, new string[] 
            {
                "QueryPet" , "HandArea" , "EnableZone"
            });

            obj.ActiveEvent += _OnEnableChipMessage;
            obj.PassiveEvent += _OnEnableChipMessage;
            _RemoveEvents.Add(obj, new Action[] { ()=>
            {
                obj.ActiveEvent -= _OnEnableChipMessage;
                obj.PassiveEvent -= _OnEnableChipMessage;
            } });
        }

        

        void _OnBattleDrawChipSupply(IDrawChip obj)
        {
            
        }

        private void _OnBattleCaptureEnergyProviderSupply(ICaptureEnergy obj)
        {
            
            
            _Command.RemotingRegister<int, EnergyGroup[]>("Capture", obj.Capture, (energy_groups) => 
            {
                _View.WriteLine("====能源x" + energy_groups.Length + "====");
                foreach (var eg in energy_groups)
                {
                    _View.Write("R:" + eg.Energy.Red + " ");
                    _View.Write("Y:" + eg.Energy.Yellow + " ");
                    _View.Write("G:" + eg.Energy.Green + " ");
                    _View.Write("P:" + eg.Energy.Power + " ");
                    _View.Write("希格斯:" + eg.Hp+ " ");
                    _View.Write("轉:" + eg.Change+ " ");
                    _View.Write("回合數:" + eg.Round+ " ");
                    _View.Write(eg.Owner == Guid.Empty ? "未奪取" : "被奪取");
                    _View.WriteLine("");

                }
            });

            _RemoveCommands.Add(obj, new string[] 
            {
                "Capture" 
            });
        }

        

        private void _OnBattleReadyCaptureEnergySupply(IReadyCaptureEnergy readycaptureenergy)
        {
            
            _Command.Register<string>("CoverCard", (param) =>
            {
                var indexs = param.Split(',');
                int[] idxs = (from index in indexs select int.Parse(index)).ToArray();
                readycaptureenergy.UseChip(idxs);
            });
            
            readycaptureenergy.UsedChipEvent += _OnUseChipResult;

            _RemoveEvents.Add(readycaptureenergy, new Action[] { () => { readycaptureenergy.UsedChipEvent -= _OnUseChipResult; } });
            _RemoveCommands.Add( readycaptureenergy , new string[] 
            {
                "CoverCard" 
            });
        }

        void _OnUseChipResult(Battle.Chip[] chips)
        {
            foreach(var chip in chips)
            {
                _View.WriteLine("激發特性 : " + chip.Name);
            }
            
        }

        
        private void _OnAdventureSupply(IAdventure adventure)
        {
            _Command.RemotingRegister<bool>("InBattle", () => 
            {
                return adventure.InBattle();
            }, (result) =>
            {
                if (result)
                {
                    _View.WriteLine("開始戰鬥.");
                }
                else
                {
                    _View.WriteLine("無法戰鬥. 可能是人數不足兩人");
                }
            });            
            _RemoveCommands.Add(adventure, new string[] 
            {
                "InBattle" 
            });
        }

        private void _OnParkingSupply(IParking parking)
        {
            
            
            Action<ActorInfomation> selectActorReturn = (actorInfomation) =>
            {
                _View.WriteLine("選擇角色 : " + actorInfomation.Id );
                _View.WriteLine("名稱 : " + actorInfomation.Name);
            };

            _Command.RemotingRegister<string, ActorInfomation>("SelectActor", parking.SelectActor, selectActorReturn);

            _RemoveCommands.Add(parking, new string[] 
            {
                "SelectActor" 
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

