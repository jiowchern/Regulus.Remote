using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Battle
{
    class Skill : Regulus.Utility.Singleton<Skill>
    {
        public delegate void OnEffect();
        Dictionary<int, OnEffect> _Skills;
        public Skill()
        {
            _Skills = new Dictionary<int, OnEffect>();            
        }

        public void SetSkill(int id, OnEffect effect)
        {
            _Skills.Add(id, effect);
        }

        public void Use(int id)
        {
            _Skills[id]();
        }
    }
    partial class Field : Regulus.Game.IFramework
    {
        public class ChipLibrary
        {
            Queue<Chip> _Chips;
                            
            public int Count { get { return _Chips.Count; } }
            public ChipLibrary()
                : this(new Chip[] { })
            {

            }
            public ChipLibrary(Chip[] chips)
            {
                _Chips = new Queue<Chip>(chips);
                Shuffle();
            }

            public void Shuffle()
            {
                var chips = _Chips.ToArray();
                int[] indexs = new int[chips.Length];
                for (int i = 0; i < chips.Length; ++i)
                {
                    indexs[i] = i;
                }
                for (int i = 0; i < chips.Length; ++i)
                {
                    var swapIndex = Regulus.Utility.Random.Next(0, chips.Length);
                    var tempChip = chips[i];
                    chips[i] = chips[swapIndex];
                    chips[swapIndex] = tempChip;
                }
                _Chips = new Queue<Chip>(chips);
            }

            public Chip Pop()
            {
                return _Chips.Dequeue();
            }

            public void Push(Chip chip)
            {
                _Chips.Enqueue(chip);
            }
        }

        public delegate void OnEnd();
        public OnEnd EndEvent;

        public delegate void OnFirst(WaittingConnectStage wcs);
        public OnFirst FirstEvent;
        BattlerInfomation[] _BattlerInfomation;
        public Field(BattlerInfomation[] battlerInfomation)
        {
            Id = Guid.NewGuid();
            _Machine = new Regulus.Game.StageMachine();
            _BattlerInfomation = battlerInfomation;            
        }
        public Guid Id { get; private set; }
        Regulus.Game.StageMachine _Machine;

        WaittingConnectStage _Begin(BattlerInfomation[] battlerInfomation)
        {
            var rcs = new WaittingConnectStage(battlerInfomation);
            rcs.ReadyEvent += _ToInitialGame;
            _Machine.Push(rcs);
            return rcs;
        }

        void _ToInitialGame(ReadyCaptureEnergyStage stage)
        {
            stage.TimeOutEvent += _ToCaptureStage;
            _Machine.Push(stage);
            
        }

        void _ToCaptureStage(Field.CaptureEnergyStage stage)
        {
            stage.TimeOutEvent += _ToEnablChipStage;
            _Machine.Push(stage);
        }

        void _ToEnablChipStage(Field.EnableChipStage stage)
        {
            stage.TimeOutEvent += _ToKillingStage;
            _Machine.Push(stage);
        }

        void _ToKillingStage(Field.KillingStage stage)
        {
            stage.EndEvent += _ToEnd;
            stage.ReadyCaptureEnergyStageEvent += _ToInitialGame;
            _Machine.Push(stage);
        }

        void _ToEnd(BattlerSide side)
        {
            EndEvent();
        }

        void Regulus.Game.IFramework.Launch()
        {
            FirstEvent(_Begin(_BattlerInfomation));
        }

        void Regulus.Game.IFramework.Shutdown()
        {
            _Machine.Termination();
        }

        bool Regulus.Game.IFramework.Update()
        {
            _Machine.Update();
            return true;
        }
    }
}
