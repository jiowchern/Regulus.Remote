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
        Regulus.Utility.TUpdater<MemberHandler> _Handlers;        
        IZone _Zone;

        public Team(IZone zone, JoinCondition join_condition)
        {
            _JoinCondidion = join_condition;
            
            _Zone = zone;
            _Handlers = new Utility.TUpdater<MemberHandler>();
        }

        bool Utility.IUpdatable.Update()
        {
            _Handlers.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {

        }

        void Framework.ILaunched.Shutdown()
        {
            _Handlers.Shutdown();
        }

        internal bool Join(Member member)
        {
            if (_JoinCondidion.Check(this))
            {                
                _QueryMap(member, _Zone.FirstMap.Name);
                return true;
            }

            return false;
        }

        

        private void _QueryMap(Member member, string map)
        {
            var handler = _QueryHandler(member);
            var stage = new MemberQueryHandler(member, _Zone, map);
            stage.ResultEvent += _JoinMap;
            handler.ChangeState(stage);
        }

        private void _JoinMap(Member member, IMap map)
        {
            var handler = _QueryHandler(member);
            var stage = new MemberMapHandler(member, map);
            stage.NextMapEvent += _QueryMap;
            handler.ChangeState(stage);
        }

        private MemberHandler _QueryHandler(Member member)
        {
            var handler = (from h in _Handlers.Objects where h.Id == member.Id select h).SingleOrDefault();
            if (handler != null)
                return handler;

            handler = new MemberHandler(member);
            _Handlers.Add(handler);
            return handler;
        }

        internal void Left(Member player)
        {           
            
            var handler = _QueryHandler(player);
            _Handlers.Remove(handler);
        }


        int JoinCondition.IResourceProvider.PlayerCount
        {
            get { return _Handlers.Count; }

        }

        internal int MemberAmount()
        {
            return _Handlers.Objects.Count();
        }
    }

    
}    
