using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public delegate void OnMapBattle(Guid battler , IBattleAdmissionTickets battle_admission_tickets);
    class Map : IMap
    {
        List<IEntity> _Entitys;
        Battle.IZone _Battle;
        public Map(Battle.IZone battle)
        {
            _Battle = battle;
            _Entitys = new List<IEntity>();
        }
        void IMap.Enter(IEntity entity)
        {
            _Entitys.Add(entity);
        }

        void IMap.Leave(IEntity entity)
        {
            _Entitys.Remove(entity);
        }

        Remoting.Value<bool> IMap.BattleRequest(Guid requester)
        {
            
            if (_Entitys.Count >= 2 )
            {
                BattleRequester br = new BattleRequester();
                int size = 0;
                foreach (var entity in _Entitys)
                {
                    BattlerInfomation battler = new BattlerInfomation();
                    battler.Id = entity.Id;
                    battler.Side = (BattlerSide)(size % 2);
                    br.Battlers.Add(battler);
                    size++;
                }

                _BroadcastBattler(_Battle.Open(br), (from battler in br.Battlers select battler.Id).ToArray());
                return true;
            }
            return false;
        }
        private void _BroadcastBattler(Remoting.Value<IBattleAdmissionTickets> value, Guid[] battlers)
        {
            value.OnValue += (battle_admission_tickets) =>
            {
                foreach (var battler in battlers)
                {
                    _BattleEvent(battler , battle_admission_tickets);
                }
            };
        }

        event OnMapBattle _BattleEvent;
        event OnMapBattle IMap.BattleResponseEvent
        {
            add { _BattleEvent += value; }
            remove { _BattleEvent -= value; }
        }
    }
}
