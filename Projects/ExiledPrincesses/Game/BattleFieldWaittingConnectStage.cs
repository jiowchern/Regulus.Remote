using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Battle
{
    partial class Field
    {
        public class WaittingConnectStage : Regulus.Game.IStage , IBattleAdmissionTickets
        {
            public class ReadyInfomation
            {
                public Regulus.Remoting.Value<IBattlerBehavior> BattleBehavior;
                public BattlerInfomation Battler;
                public Pet Pet;
            }

            List<ReadyInfomation> _ReadyInfomations;
            
            public delegate void OnReady(ReadyCaptureEnergyStage stage);
            event OnReady _ReadyEvent;
            public event OnReady ReadyEvent { add { _ReadyEvent += value; } remove { } }
            public delegate void OnTimeOut();
            event OnTimeOut _TimeOutEvent;
            public event OnTimeOut TimeOutEvent { add { _TimeOutEvent += value; } remove { } }

            int _Count;
            Regulus.Utility.TimeCounter _Timeout;
            public WaittingConnectStage(BattlerInfomation[] battlerInfomation)
            {
                _Count = 0;
                _ReadyInfomations = new List<ReadyInfomation>();  
                foreach(var battrler in battlerInfomation)
                {
                    _ReadyInfomations.Add(new ReadyInfomation() { Battler = battrler });
                }

                _Timeout = new Utility.TimeCounter();
                
            }
            
            void Regulus.Game.IStage.Enter()
            {
                                
            }

            void Regulus.Game.IStage.Leave()
            {
                
            }

            void Regulus.Game.IStage.Update()
            {
                if (_Count == _ReadyInfomations.Count)
                {
                    if (_ReadyEvent != null)
                    {
                        var cl = _GenerateCommonChipSet();
                        List<Player> players = new List<Player>();

                        int side = 0;
                        foreach (var ri in _ReadyInfomations)
                        {
                            var player = new Player(ri.Pet);
                            player.Initial(cl , (BattlerSide)(side++ % 2));
                            
                            ri.BattleBehavior.SetValue(player);
                            players.Add(player);
                        }

                        _ReadyEvent(new ReadyCaptureEnergyStage(players.ToArray(), cl , 10));
                    }
                    _ReadyEvent = null;
                }
                else
                {
                    var couuent = new System.TimeSpan(_Timeout.Ticks);
                    if (couuent.TotalSeconds > 1000)
                    {
                        if (_TimeOutEvent != null)
                        {
                            _TimeOutEvent();
                        }
                        _TimeOutEvent = null;
                    }
                }
                
            }

            private ChipLibrary _GenerateCommonChipSet()
            {
                // 築巢
                Chip chip1 = new Chip();
                chip1.Name = "築巢";
                chip1.Red = new int[] { 0, 0, 0 };
                chip1.Yellow = new int[] { 1, 1, 2 };
                chip1.Green = new int[] { 0, 0, 0 };
                chip1.Power = new int[] { 0, 0, 0 };
                chip1.Initiatives = new int[] { 4 };
                chip1.Passives = new int[] { 2 };

                // 下風突襲
                Chip chip2 = new Chip();
                chip2.Name = "下風突襲";
                chip2.Red = new int[] { 1, 1, 1 };
                chip2.Yellow = new int[] { 0, 0, 0 };
                chip2.Green = new int[] { 0, 0, 1 };
                chip2.Power = new int[] { 0, 0, 0 };
                chip2.Initiatives = new int[] { 3 };
                chip2.Passives = new int[] { 1 };

                
                ChipLibrary cl = new ChipLibrary();
                for (int i = 0; i < 500; ++i )
                {
                    cl.Push(chip2);
                    cl.Push(chip1);
                }
                cl.Shuffle();
                return cl;
            }

            Regulus.Remoting.Value<IBattlerBehavior> IBattleAdmissionTickets.Visit(Pet pet)
            {
                Regulus.Remoting.Value<IBattlerBehavior> rce = new Remoting.Value<IBattlerBehavior>();
                ReadyInfomation ri = (from readyInfomation in _ReadyInfomations where readyInfomation.Battler.Id == pet.Owner && readyInfomation.Pet == null select readyInfomation).FirstOrDefault();
                if (ri != null)
                {                                     
                    ri.Pet = pet;
                    ri.BattleBehavior = rce;                    
                    _Count++;                    
                }
                return rce;
            }
        }
        
    }
}
