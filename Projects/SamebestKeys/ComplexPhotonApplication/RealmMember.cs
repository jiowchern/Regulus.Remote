
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{

    partial class Realm
    {
        
        public class Member : ITraversable 
        {

            public delegate void OnMigrate();
            public event OnMigrate MigrateEvent;

            public delegate IMap OnCross(string map);
            public event OnCross CrossEvent;

            public delegate void OnTraversable(ITraversable traversable);
            public event OnTraversable BeginTraversableEvent;
            public event OnTraversable EndTraversableEvent;

            private Player _Player;
            IMap _Map;

            public Member(Player player)
            {
                this._Player = player;
            }
            internal void Left()
            {
                _Player.SetMap(null);
                var ability = _Player.FindAbility<ICrossAbility>();
                ability.MoveEvent -= _OnMove;

                if (_Map != null)
                {
                    _Map.Left(_Player);
                    _Map = null;
                }
            }
            internal void Into(string map)
            {
                _Map = CrossEvent(map);
                BeginTraversableEvent(this);                
            }
            internal void Into(IMap map)
            {
                Left();
                _Map = map;
                _Map.Into(_Player);
                _Player.SetMap(map.Name);

                var ability = _Player.FindAbility<ICrossAbility>();
                ability.MoveEvent += _OnMove;
            }

            private void _OnMove(string target_map, Types.Vector2 target_position)
            {
                Into(target_map);
            }

            internal void ToMap()
            {
                EndTraversableEvent(this);                                
                Into(_Map);
            }

            public Guid Id { get { return _Player.Id; } }

            Remoting.Value<Serializable.CrossStatus> ITraversable.GetStatus()
            {
                return new Serializable.CrossStatus() { TargetMap = _Map.Name };
            }

            void ITraversable.Ready()
            {
                ToMap();
            }
        }
    }
}
