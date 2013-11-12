using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Battle
{
    partial class Field
    {
        public class CaptureEnergyStage : Regulus.Game.IStage
        {
            public class Capturer : ICaptureEnergy
            {
                public Player Player;
                public event Func<Player, int, bool> CaptureEvent;
                int _Difficulty;
                EnergyGroup[] _EnergyGroup;
                public bool Done { get; private set; }
                public Capturer(Player player, int difficulty )
                {
                    
                    _Difficulty = difficulty;
                    Player = player;
                }

                public void Initial(EnergyGroup[] energy_group)
                {
                    _EnergyGroup = energy_group;
                }

                public void Release()
                { 

                }

                public void Capture(int idx)
                {
                    CaptureEvent(Player, idx);
                }
                

                Remoting.Value<EnergyGroup[]> ICaptureEnergy.Capture(int idx)
                {
                    if (Done == false && CaptureEvent(Player, idx))
                    {
                        Done = true;                        
                    }
                    return _EnergyGroup;
                }
            }

            public delegate void OnTimeOut(EnableChipStage stage);
            public event OnTimeOut TimeOutEvent;
            Regulus.Utility.TimeCounter _Timeout;
            
            private ChipLibrary _ChipLibrary;
            private int _RoundCount;            

            EnergyGroup[] _Groups;

            List<Capturer> _Capturers;
            public CaptureEnergyStage(List<Capturer> capturers, ChipLibrary chiplibrary, int roundcount)
            {
                _Capturers = capturers;             
                this._ChipLibrary = chiplibrary;
                this._RoundCount = roundcount;
                _Groups = new EnergyGroup[_Capturers.Count + 1];
            }

            bool _OnCapture(Player player, int idx)
            {
                if (idx < _Groups.Length && _Groups[idx].Owner == Guid.Empty)
                {
                    _IncEnergy(_Groups[idx].Energy.Red, player.Energy.IncRed);
                    _IncEnergy(_Groups[idx].Energy.Green, player.Energy.IncGreen);
                    _IncEnergy(_Groups[idx].Energy.Yellow, player.Energy.IncYellow);
                    _IncEnergy(_Groups[idx].Energy.Power, player.Energy.IncPower);

                    _Groups[idx].Owner = player.Pet.Owner;
                    return true;
                }
                return false;
            }

            private void _IncEnergy(int count, Func<bool> func)
            {
                for (int i = 0; i < count; ++i)
                {
                    func();
                }
            }
            void Regulus.Game.IStage.Enter()
            {                
                for (int i = 0; i < _Groups.Length; ++i )
                {
                    var energy = new Energy(3);
                    var eg = new EnergyGroup() { Energy = energy, Round = Regulus.Utility.Random.Next(0, 3) } ;
                    Func<bool>[] incs1 = 
                    {
                        energy.IncGreen , energy.IncRed , energy.IncYellow 
                    };
                    incs1[Regulus.Utility.Random.Next(0, incs1.Length)]();
                    incs1[Regulus.Utility.Random.Next(0, incs1.Length)]();

                    Func<bool>[] incs2 = 
                    {
                        energy.IncPower , ()=>{eg.Hp = 1; return true;} , ()=>{eg.Change = 1; return true;}
                    };

                    incs2[Regulus.Utility.Random.Next(0, incs2.Length)]();
                    _Groups[i] = eg ;
                }

                foreach (var capture in _Capturers)
                {
                    capture.Player.OnSpawnCaptureEnergy(capture);
                    capture.Initial(_Groups);
                    capture.CaptureEvent += _OnCapture;
                }

                _Timeout = new Utility.TimeCounter();

                
            }

            void Regulus.Game.IStage.Leave()
            {
                foreach (var capture in _Capturers)
                {
                    capture.CaptureEvent -= _OnCapture;
                    capture.Player.OnUnspawnCaptureEnergy();
                    capture.Release();
                }
            }


            
            
            void Regulus.Game.IStage.Update()
            {                
                var couuent = new System.TimeSpan(_Timeout.Ticks);
                if (couuent.TotalSeconds > 1000)
                {
                    for (int i = 0; i < _Groups.Length; ++i)
                    {
                        var group = _Groups[i];
                        if (group != null)
                        {
                            foreach (var cap in _Capturers)
                            {
                                if (cap.Done == false)
                                {
                                    cap.Capture(i);
                                }
                            }
                        }
                    }
                }
                if((from d in _Capturers where d.Done == true select d).Count() == _Capturers.Count())
                {                    
                    _ToNext( (from g in _Groups where g != null select g.Round).FirstOrDefault() );
                }
            }

            private void _ToNext(int release_round)
            {
                if (TimeOutEvent != null)
                {
                    TimeOutEvent(new EnableChipStage((from c in _Capturers select new EnableChipStage.Battler(c.Player)).ToArray(), _ChipLibrary, _RoundCount - release_round));
                }
                TimeOutEvent = null;
            }
        }
    }
    
}
