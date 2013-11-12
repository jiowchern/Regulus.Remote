using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Battle
{
    partial class Field
    {
        public class EnableChipStage : Regulus.Game.IStage
        {
            public class Battler : IEnableChip
            {
                ChipLibrary _Common;
                public Player Player { get; private set; }
                int _Speed;
                int _Attack;
                public int Speed { get { return _Speed + Player.Speed;  } }
                Battler[] _Battlers;
                public Battler(Player player)
                {
                    Player = player;
                    
                }

                internal void Initial(int speed , ChipLibrary common)
                {
                    _Speed = speed;
                    
                    _Common = common;
                }
                public void SetBattlers(Battler[] battlers)
                {
                    _Battlers = battlers;
                }
                public bool Done { get 
                {
                    if (_Time != null)
                        return new System.TimeSpan(_Time.Ticks).TotalSeconds > 600 || _Done;
                    return false;
                } }

                Regulus.Utility.TimeCounter _Time;
                internal void CloseEnable()
                {
                    Player.OnUnspawnEnableChip();
                }

                internal void OpenEnable()
                {
                    Player.OnSpawnEnableChip(this);
                    _Time = new Utility.TimeCounter();
                }

                Regulus.Remoting.Value<bool> IEnableChip.Enable(int index)
                {
                    
                    if (index < Player.EnableChips.Length && Player.EnableChips[index] != null)
                    {
                        var chip = Player.EnableChips[index];
                        Player.EnableChips[index] = null;
                        int idx = _Battlers.Length / 2 - 1;
                        if (Player.Energy.Consume(chip.Red[idx], chip.Yellow[idx], chip.Green[idx], chip.Power[idx]))
                        {
                            foreach (var effect in chip.Initiatives)
                            {
                                _ExecuteSpell(this, effect, _Battlers);
                            }
                            Player.RecycleChip.Push(chip);
                            Player.RecycleChip.Push(_GenerateChip());
                            return true;
                        }
                        else
                        {
                            Player.EnableChips[index] = chip;
                        }
                        
                    }
                    return false;
                }

                private Chip _GenerateChip()
                {
                    return _Common.Pop();                    
                }

                internal void StimulatePassive()
                {
                    foreach (var ec in Player.EnableChips)
                    {
                        if (ec != null)
                        {
                            foreach (var effect in ec.Passives)
                            {                                
                                _ExecuteSpell(this, effect, _Battlers);
                            }
                        }
                        
                    }
                }

                private void _ExecuteSpell(Battler battler, int effect, Battler[] battlers)
                {
                    if (effect == 1)
                    {
                        if (battler.Player.Energy.Green == 0)
                        {
                            battler.AddAttack(6);
                            Player.OnPassiveMessage(battler.Name + "因為[下風突襲]能力啟用攻擊力+6");                            
                        }
                        
                    }
                    if (effect == 2)
                    {
                        battler.Player.Hp += 3;
                        Player.OnPassiveMessage(battler.Name + "因為[築巢]能力啟用希格斯+3");                            
                    }

                    if (effect == 3)
                    { 
                        
                        foreach(var target in battlers)
                        {
                            if (target.Side != battler.Side)
                            {
                                int damage = battler.Attack + 10;
                                target.SubGreen();
                                target.SubGreen();
                                target.Injuries(damage);


                                Player.OnActiveMessage(battler.Name + "發動下風突襲 > " + target.Name + "損失" + damage + "希格斯");
                            }
                            
                        }
                    }
                    if (effect == 4)
                    {
                        battler.TurnonProtection();
                        battler.IncRed();
                        Player.OnActiveMessage(battler.Name + "發動築巢獲得保護狀態");
                    }
                }
                int _Protection = 1;
                private void TurnonProtection()
                {
                    _Protection = 2;
                }
                private void TurnoffProtection()
                {
                    _Protection = 1;
                }

                private void Injuries(int damage)
                {
                    Player.Hp -= (damage / _Protection);
                }
                
                private void IncRed()
                {
                    Player.Energy.IncRed();                    
                }

                private void SubGreen()
                {
                    Player.Energy.SubGreen();
                }

                private void AddAttack(int p)
                {
                    _Attack += p;
                }



                public BattlerSide Side { get { return Player.Side;  } }

                public int Attack { get { return _Attack + Player.Attack; } }

                public int Hp { get { return Player.Hp; } }


                bool _Done;
                void IEnableChip.Done()
                {
                    _Done = true;
                }
                

                public string Name { get { return Player.Pet.Name; } }                

                Remoting.Value<BattleSpeed[]> IEnableChip.QuerySpeeds()
                {
                    return (from battler in _Battlers select new BattleSpeed() { Name = battler.Name, Speed = battler.Speed }).ToArray();
                }
            }

            public delegate void OnTimeOut(KillingStage stage);
            public event OnTimeOut TimeOutEvent;
           
            private ChipLibrary _ChipLibrary;
            private int _RoundCount;
            Battler[] _Battlers;
            Queue<Battler> _Standby;
            public EnableChipStage(Battler[] battlers, ChipLibrary chiplibrary, int roundcount)
            {
                _Battlers = battlers;
                
                this._ChipLibrary = chiplibrary;
                this._RoundCount = roundcount;
            }

            void Regulus.Game.IStage.Enter()
            {                
                Queue<int> signs = new Queue<int>(_BuildSigns());

                foreach (var battler in _Battlers)
                {
                    battler.Initial(signs.Dequeue(), _ChipLibrary);
                    battler.StimulatePassive();
                }

                _Battlers = (from battler in _Battlers where battler.Hp > 0 orderby battler.Speed descending select battler).ToArray();


                foreach (var battler in _Battlers)
                {
                    battler.SetBattlers(_Battlers);                    
                }
                _Standby = new Queue<Battler>(_Battlers);

                _Current = _NextBattler(_Standby);
                _Current.OpenEnable();
            }

            private static int[] _BuildSigns()
            {
                int[] signs = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                for (int i = 0; i < signs.Length; ++i)
                {
                    var idx1 = i;
                    var idx2 = Regulus.Utility.Random.Next(0, signs.Length);
                    var s = signs[idx1];
                    signs[idx1] = signs[idx2];
                    signs[idx2] = s;
                }
                return signs;
            }

            void Regulus.Game.IStage.Leave()
            {
                
            }


            Battler _Current;
            void Regulus.Game.IStage.Update()
            {
                if (_Current.Done)
                {
                    _Current.CloseEnable();
                    var battler = _NextBattler(_Standby);
                    if (battler != null)
                    {
                        battler.OpenEnable();
                    }
                    
                    _Current = battler;                    
                }

                if (_Current == null)
                {
                    
                    if (TimeOutEvent != null)
                        TimeOutEvent(new KillingStage((from battler in _Battlers select battler.Player).ToArray() , _ChipLibrary , _RoundCount - 1 ) );
                    TimeOutEvent = null;
                }
            }

            private Battler _NextBattler(Queue<Battler> standby)
            {
                if (standby.Count > 0)
                {
                    var battler = standby.Dequeue();
                    
                    return battler;
                }
                return null;
            }
        }
    }
}
