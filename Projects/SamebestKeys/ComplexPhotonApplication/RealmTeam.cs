using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Dungeons
{
    using Extension;
    class Team : Regulus.Utility.IUpdatable, JoinCondition.IResourceProvider
    {
        JoinCondition _JoinCondidion;
        Regulus.Utility.Updater _Updater;
        List<Member> _Members;
        IZone _Zone;

        public Team(IZone zone, JoinCondition join_condition)
        {
            _JoinCondidion = join_condition;
            _Members = new List<Regulus.Project.SamebestKeys.Dungeons.Member>();
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

        internal bool Join(Member member)
        {
            if (_JoinCondidion.Check(this))
            {
                member.MigrateEvent += _OnMigrate;
                member.CrossEvent += _OnCross;

                member.Into(_Zone.FirstMap.Name);
                _Members.Add(member);
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

        internal void Left(Player player)
        {
            var s = (from star in _Members where star.Id == player.Id select star).SingleOrDefault();
            s.Left();

            s.MigrateEvent -= _OnMigrate;
            s.CrossEvent -= _OnCross;
            _Members.Remove(s);
        }


        int JoinCondition.IResourceProvider.PlayerCount
        {
            get { return _Members.Count; }

        }
    }
}    
