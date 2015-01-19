
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Dungeons
{    
    class MemberMapHandler : Regulus.Utility.IStage
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

        void Utility.IStage.Enter()
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

        private void _OnCross(string target_map, CustomType.Vector2 target_position)
        {
            _Map.Left(_Member.Player);            
            _NextMap = target_map;
        }

        void Utility.IStage.Leave()
        {
            _Map.LeftDoneEvent -= _LeftDone;
            _Map.Left(_Member.Player);            
        }

        void Utility.IStage.Update()
        {
            
        }
    }
    class MemberQueryHandler : Regulus.Utility.IStage , ITraversable
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
        void Utility.IStage.Enter()
        {
            if (_Member.BeginTraversable != null)
                _Member.BeginTraversable(this); 
        }

        

        void Utility.IStage.Leave()
        {
            if (_Member.BeginTraversable != null)
                _Member.EndTraversable(this);
        }

        void Utility.IStage.Update()
        {            
        }

        Remoting.Value<Serializable.CrossStatus> ITraversable.GetStatus()
        {
            return new Serializable.CrossStatus() { TargetMap = _Map };
        }

        void ITraversable.Ready()
        {            

            var map = _Zone.Find(_Map);
            ResultEvent(_Member, map);

        }
    }

    class MemberHandler : Regulus.Utility.IUpdatable
    {

        Member _Member;
        Regulus.Utility.StageMachine _Machine;
        public Guid Id { get { return _Member.Id; } }

        public MemberHandler(Member member)
        {
            this._Member = member;
            _Machine = new Utility.StageMachine();
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

        internal void ChangeState(Regulus.Utility.IStage stage)
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
