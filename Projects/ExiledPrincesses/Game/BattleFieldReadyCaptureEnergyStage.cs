
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Battle
{
    partial class Field
    {
        public class ReadyCaptureEnergyStage : Regulus.Game.IStage
        {
            int _RoundCount;
            private ChipLibrary _ChipLibrary;
            private List<Player> _Players;
            class Decided : IReadyCaptureEnergy
            {
                public Player Owner;
                public int ChipCount;
                public void OnChips(Chip[] chips)
                {
                    ChipCount = chips.Length;
                    IsDecided = true;
                }
                public bool IsDecided;

                public event Action<Chip[]> UsedChipEvent;

                void IReadyCaptureEnergy.UseChip(int[] chip_indexs)
                {
                    if (IsDecided == false)
                    {
                        var chips = Owner.UseChip(chip_indexs);
                        OnChips(chips);
                        _UsedChipEvent(chips);
                    }                    
                }

                event Action<Chip[]> _UsedChipEvent;
                event Action<Chip[]> IReadyCaptureEnergy.UsedChipEvent
                {
                    add { _UsedChipEvent += value; }
                    remove { _UsedChipEvent -= value; }
                }
            }
            List<Decided> _Decideds;

            public delegate void OnTimeOut(CaptureEnergyStage stage);
            public event OnTimeOut TimeOutEvent;
            Regulus.Utility.TimeCounter _Timeout;            

            public ReadyCaptureEnergyStage(Player[] players, ChipLibrary common_chips , int round_count)
            {
                _Players = new List<Player>(players);
                _ChipLibrary = common_chips;
                _RoundCount = round_count;
            }

            void Regulus.Game.IStage.Enter()
            {
                _Timeout = new Utility.TimeCounter();
                _Decideds = new List<Decided>();
                foreach(var player in _Players)
                {                    
                    
                    var decided = new Decided() { Owner = player };                    
                    
                    player.OnSpawnReadyCaptureEnergy(decided);
                    _Decideds.Add(decided);
                }
            }

            void Regulus.Game.IStage.Leave()
            {
                foreach (var decided in _Decideds)
                {
                    
                }
            }

            void Regulus.Game.IStage.Update()
            {
                var couuent = new System.TimeSpan(_Timeout.Ticks);
                if (couuent.TotalSeconds > 1000 || (from d in  _Decideds where d.IsDecided == true select d).Count() == _Players.Count())
                {
                    if (TimeOutEvent != null)
                    {
                        var captuters = from player in _Decideds select new CaptureEnergyStage.Capturer(player.Owner, Player.UsedCardCount - player.ChipCount);

                        TimeOutEvent(new CaptureEnergyStage(captuters.ToList(), _ChipLibrary, _RoundCount));
                    }
                    TimeOutEvent = null;
                }
            }

            

            
        }
    }
    
}
