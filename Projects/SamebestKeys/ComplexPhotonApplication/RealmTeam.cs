using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    using Extension;
    partial class Realm
    {
        class Team : Regulus.Utility.IUpdatable , JoinCondition.IResourceProvider
        {
            JoinCondition _JoinCondidion;
            Regulus.Utility.Updater _Updater;
            List<Member> _Stars;
            IZone _Zone;
            public int Count { get { return _Stars.Count; } }

            public Team(IZone zone, JoinCondition join_condition)
            {
                _JoinCondidion = join_condition;
                _Stars = new List<Member>();
                _Zone = zone;
                _Updater = new Utility.Updater();            
            }

            bool Utility.IUpdatable.Update()
            {
                _Updater.Update();                
                return true;
            }

            void Framework.ILaunched.Launch()
            {
                
            }

            void Framework.ILaunched.Shutdown()
            {
                _Updater.Shutdown();
            }

            internal Remoting.Value<bool> Join(Player player)
            {
                if (_JoinCondidion.Check(this))
                {
                    var member = new Member(player);
                    member.MigrateEvent += _OnMigrate;
                    member.CrossEvent += _OnCross;

                    member.Into(_Zone.FirstMap);
                    _Stars.Add(member);
                    return true;
                }

                return false;
            }

            private IMap _OnCross(string map_name)
            {
                return _Zone.Find(map_name);                
            }

            private void _OnMigrate()
            {
                // todo : 遷移

            }

            private IMap _CurrentMap(IMap[] maps)
            {
                // todo : 找出第一張或者是活動中的地圖
                return maps[0];
            }

            

            internal void Left(Player player)
            {
                var s = (from star in _Stars where star.Id == player.Id select star).SingleOrDefault();
                s.Left();

                s.MigrateEvent -= _OnMigrate;
                s.CrossEvent -= _OnCross; 
                _Stars.Remove(s); 
            }


            int JoinCondition.IResourceProvider.PlayerCount
            {
                get { return _Stars.Count; }
                
            }
        }
    }
    
}
