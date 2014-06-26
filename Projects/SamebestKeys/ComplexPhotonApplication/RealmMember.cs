
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Dungeons
{    
    class MemberMapHandler : Regulus.Game.IStage
    {
        private Member _Member;
        private IMap _Map;

        string _NextMap;

        public delegate void OnNextMap(Member member , string map);
        public event OnNextMap NextMapEvent;
        

        public MemberMapHandler(Member member, IMap map)
        {            
            this._Member = member;
            this._Map = map;
        }

        void Game.IStage.Enter()
        {
            
            var crossAbility = _Member.Player.FindAbility<ICrossAbility>();
            crossAbility.MoveEvent += _OnCross;

            _Map.LeftDoneEvent += _LeftDone;

            var point = GameData.Instance.FindMap(_Map.Name).Born;
            var x = Regulus.Utility.Random.NextFloat(point.Left, point.Right);
            var y = Regulus.Utility.Random.NextFloat(point.Top, point.Bottom);


            _Member.Player.SetPosition(x, y);
            _Map.Into(_Member.Player);
        }

        private void _LeftDone(Guid id)
        {
            if (_Member.Id == id && _NextMap != null)
            {
                _Member.Player.ClearField();
                NextMapEvent(_Member, _NextMap);
            }
        }

        private void _OnCross(string target_map, Types.Vector2 target_position)
        {
            _Map.Left(_Member.Player);            
            _NextMap = target_map;
        }

        void Game.IStage.Leave()
        {
            _Map.LeftDoneEvent -= _LeftDone;
            _Map.Left(_Member.Player);            
        }

        void Game.IStage.Update()
        {
            
        }
    }
    class MemberQueryHandler : Regulus.Game.IStage , ITraversable
    {
        Member _Member;
        string _Map;
        IZone _Zone;

        public delegate void OnResult(Member member , IMap map);
        public event OnResult ResultEvent;

        public MemberQueryHandler(Member member, IZone zone, string map)
        {
            _Member = member;
            _Zone = zone;
            _Map = map;
        }
        void Game.IStage.Enter()
        {
            _Member.BeginTraversable(this); 
        }

        

        void Game.IStage.Leave()
        {
        
        }

        void Game.IStage.Update()
        {            
        }

        Remoting.Value<Serializable.CrossStatus> ITraversable.GetStatus()
        {
            return new Serializable.CrossStatus() { TargetMap = _Map };
        }

        void ITraversable.Ready()
        {
            _Member.EndTraversable(this);

            var map = _Zone.Find(_Map);
            ResultEvent(_Member, map);

        }
    }

    class MemberHandler : Regulus.Utility.IUpdatable
    {

        Member _Member;
        Regulus.Game.StageMachine _Machine;
        public Guid Id { get { return _Member.Id; } }

        public MemberHandler(Member member)
        {
            this._Member = member;
            _Machine = new Game.StageMachine();
        }
        bool Utility.IUpdatable.Update()
        {
            _Machine.Update();
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            
        }

        void Framework.ILaunched.Shutdown()
        {
            _Machine.Termination();
        }

        internal void ChangeState(Regulus.Game.IStage stage)
        {
            _Machine.Push(stage);
        }
    }
    class Member 
    {
        private Player _Player;

        public delegate void OnTraversable(ITraversable traversable);
        public OnTraversable BeginTraversable;
        public OnTraversable EndTraversable;

        public Member(Player player)
        {
            this._Player = player;
        }


        public Guid Id { get { return _Player.Id; } }

        public Player Player { get { return _Player; } }
    }
}
