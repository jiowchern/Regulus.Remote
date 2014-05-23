
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

    partial class Realm
    {
        class Member
        {
            public delegate void OnMigrate();
            public event OnMigrate MigrateEvent;

            public delegate IMap OnCross(string map);
            public event OnCross CrossEvent;

            private Player _Player;
            IMap _Map;

            public Member(Player player)
            {
                this._Player = player;
            }
            internal void Left()
            {

                var ability = _Player.FindAbility<ICrossAbility>();
                ability.MoveEvent -= _OnMove;

                if (_Map != null)
                {
                    _Map.Left(_Player);
                    _Map = null;
                }
            }
            internal void Into(IMap map)
            {
                Left();
                _Map = map;
                _Map.Into(_Player);

                var ability = _Player.FindAbility<ICrossAbility>();
                ability.MoveEvent += _OnMove;
            }

            private void _OnMove(string target_map, Types.Vector2 target_position)
            {
                var map = CrossEvent(target_map);
                if (map != null)
                    Into(map);
            }

            public Guid Id { get { return _Player.Id; } }
        }
    }
}
