using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public class Team : ITeam
    {
        public class Member
        {
            public Member(Team owner, ITeammate teammate)
            {
                Owner = owner;
                Teammate = teammate;
            }
            public Team Owner { get; private set; }
            public ITeammate Teammate { get; private set; }
        }
        
        private ITeammate[] _Front;
        private ITeammate[] _Back;

        Platoon _Platoon { get; set; }
        TeamSide _Side;
        public Team(TeamSide side, Platoon platoon)
        {
            _Side = side;
            _Platoon = platoon;
            _Platoon.SetSide(side);
            
            _Strategys = new int[(int)Strategy.Count];

            if (_Platoon.Formation == Contingent.FormationType.Auxiliary)
            {

                _Front = new ITeammate[] { _Platoon.Teammates[0] };
                int i = 0;
                _Back = (from teammate in _Platoon.Teammates
                        let idx = i++
                        where idx > 0
                        select teammate).ToArray();
            }

            if (_Platoon.Formation == Contingent.FormationType.Defensive)
            {
                _Back = new ITeammate[] { _Platoon.Teammates[0] };
                int i = 0;
                _Front = (from teammate in _Platoon.Teammates
                         let idx = i++
                         where idx > 0
                         select teammate).ToArray();
            }

        }

        public Member[] Members { get { return (from t in _Platoon.Teammates select new Member(this, t)).ToArray(); } }
        int[] _Strategys;
        internal void AddStrategy(Strategy strategy)
        {
            _Strategys[(int)strategy] ++ ;
        }

        internal void AddStrategy(Strategy strategy,int amount)
        {
            _Strategys[(int)strategy] += amount;
        }

        internal bool Consumer(int[] strategys)
        {
            int meet = 0;
            for (int i = 0; i < _Strategys.Length; ++i)
            {
                if (_Strategys[i] >= strategys[i])
                {
                    meet++;
                }
            }

            if (meet == _Strategys.Length)
            {
                for (int i = 0; i < _Strategys.Length; ++i)
                {
                    _Strategys[i] -= strategys[i];
                }
                return true;
            }
            return false;
        }

        internal ITeammate[] GetFront()
        {
            var count = (from teammate in _Front where teammate.Hp > 0 select teammate).Count();
            if (count > 0)
                return _Front;
            return _Back;
        }

        int[] ITeam.Strategys
        {
            get { return _Strategys; }
        }

        internal void InitialBroadcast(Team[] teams)
        {
            _Platoon.BattleBegins();
            _Platoon.SetTeams(teams);
            foreach (var teammates in from t in teams where t != this select t._Platoon.Teammates)
                _Platoon.SetEnemys(teammates);
        }

        internal void FinalialBroadcast()
        {
            
            _Platoon.SetEnemys(new ITeammate[0]);
            _Platoon.SetTeams(new ITeam[0]);

            _Platoon.EndBegins();
        }

        
        TeamSide ITeam.Side
        {
            get { return _Side; }
        }
    }

    partial class Combat
    {
        public delegate void OnWinner(Team team);
        public event OnWinner WinnerEvent;
        public delegate void OnDraw();
        public event OnDraw DrawEvent;
        Regulus.Game.StageMachine _StageMachine;
        Team[] _Teams;
        CommonSkillSet _CommonSkillSet;
        
        internal void Update()
        {
            _StageMachine.Update();
        }
        
        internal void Initial(Team team1, Team team2)
        {
            _CommonSkillSet = new CommonSkillSet();
            _CommonSkillSet.Shuffle();
            _CommonSkillSet.EmptyEvent += () => { DrawEvent(); };
            _Teams = new Team[] { team1 , team2 };
            _StageMachine = new Regulus.Game.StageMachine();


            _Take(_Teams , _CommonSkillSet);
            _InitialBroadcasting(_Teams);
            
            _ToStrategy(_Teams);            
        }
        internal void Finial()
        {
            _ReleaseBroadcasting(_Teams);
            _StageMachine.Termination();
        }

        private void _Take(Team[] teams, CommonSkillSet common_skill_set)
        {                        
            foreach (var team in teams)
            {                 
                foreach(var member in team.Members)
                {
                    member.Teammate.Take(common_skill_set);
                }
            }
        }

        private void _ToStrategy(Team[] teams)
        {
            var stage = new StrategyStage(teams);
            stage.DoneEvent += _ToAction;
            _StageMachine.Push(stage);
        }

        private void _ToAction()
        {
            var stage = new ActionStage(_Teams, _CommonSkillSet);
            stage.DoneEvent += () => { _ToStrategy(_Teams); };
            stage.EliminateEvent += _EliminateTeam;
            _StageMachine.Push(stage);
        }
        private void _EliminateTeam(Team[] eliminates)
        {
            List<Team> teams = _Teams.ToList();
            foreach (var eliminate in eliminates)
            {
                teams.Remove(eliminate);
            }
            if (teams.Count == 1)
            {
                WinnerEvent(teams.ToArray()[0]);
            }
        }

        


        private void _InitialBroadcasting(Team[] teams)
        {            
            foreach (var team in teams)
            {
                team.InitialBroadcast(teams);                
            }
        }

        private void _ReleaseBroadcasting(Team[] teams)
        {        
            foreach (var team in teams)
            {
                team.FinalialBroadcast();
            }
        }

        
    }
    partial class Combat
    {
        class ActionStage : Regulus.Game.IStage
        {
            class Activists : Regulus.Utility.IUpdatable
            {
                const float _IdleTime = 2.0f;
                public delegate void OnDone();
                public event OnDone DoneEvent;
                private Team.Member _Member;
                Team[] _Targets;
                Regulus.Utility.IndependentTimer _Timer;
                CommonSkillSet _CommonSkillSet;
                public Activists(Team.Member member, Team[] targets, CommonSkillSet common_skill_set)
                {
                    _CommonSkillSet = common_skill_set;
                    _Targets = targets;
                    this._Member = member;
                }
                
                public bool Update()
                {
                    
                    _Timer.Update();
                    return true;
                }

                public void Launch()
                {
                    Skill.Effect[] effects = _Member.Teammate.GetActivitiesEffects(_Member.Owner, _CommonSkillSet);

                    float waitTime = _UseEffects(effects) + _IdleTime;
                    _Timer = new Utility.IndependentTimer(TimeSpan.FromSeconds(waitTime), _Done );
                    
                }

                private void _Done(long obj)
                {
                    DoneEvent();
                }


                private float _UseEffects(Skill.Effect[] effects)
                {
                    float waitTime = 0.0f;
                    foreach (var effect in effects)
                    {                        
                        waitTime += effect.Duration;
                        effect.Action(_Member, _Targets);                        
                    }
                    return waitTime;
                }

                public void Shutdown()
                {
                    
                }
            }

            Activists _Current;
            Queue<Activists> _Activists;
            
            public delegate void OnDone();
            public event OnDone DoneEvent;
            public delegate void OnEliminate(Team[] teams);
            public event OnEliminate EliminateEvent;
            Team[] _Teams;
            
            public ActionStage(Team[] teams, CommonSkillSet common_skill_set)
            {
                
                _Teams = teams;
                _Activists = new Queue<Activists>(_GetSurvivor(teams, common_skill_set));
            }

            void Regulus.Game.IStage.Enter()
            {
                _Current = _Activists.Dequeue();                
                _Current.DoneEvent += _Next;
                _Current.Launch();
            }

            private void _Next()
            {                
                if (_Activists.Count > 0)
                {
                    _Current.Shutdown();
                    _Current = _Activists.Dequeue();
                    _Current.DoneEvent += _Next;
                    _Current.Launch();
                }
                else
                {
                    _Judge();
                }
                
            }

            private void _Judge()
            {
                var eliminateTeams = (from t in _Teams 
                            let totalHp = (from member in t.Members select member.Teammate.Hp).Sum()
                            where totalHp <= 0
                            select t).ToArray();

                if (eliminateTeams.Length > 0)
                    EliminateEvent(eliminateTeams);
                else
                    DoneEvent();
            }

            void Regulus.Game.IStage.Leave()
            {
                _Current.Shutdown();
            }
            
            void Regulus.Game.IStage.Update()
            {
                _Current.Update();
            }

            private Activists[] _GetSurvivor(Team[] teams, CommonSkillSet common_skill_set)
            {
                List<Team.Member> teammates = new List<Team.Member>();
                foreach (var team in teams)
                {
                    teammates.AddRange(from member in team.Members where member.Teammate.Hp > 0 select member);
                }
                teammates = teammates.OrderBy(t => t.Teammate.Dex + Regulus.Utility.Random.Next(-2, 2)).ToList();

                return (from t in teammates select new Activists(t, (from team in teams where t.Owner != team select team).ToArray(), common_skill_set)).ToArray();
            }
        }
    }
    partial class Combat
    {
        class StrategyStage : Regulus.Game.IStage
        {
            public delegate void OnDone();
            public event OnDone DoneEvent;
            private Team[] _Teams;

            public StrategyStage(Team[] teams)
            {                
                this._Teams = teams;

            }

            void Regulus.Game.IStage.Enter()
            {
                var strategys = _Generate();
                _Snatch(strategys);
            }

            private void _Snatch(Strategy[] strategys)
            {
                var members = _GetSurvivor(_Teams);
                int i = 0;
                while(true)
                {
                    var memeber = members[i];

                    i++;
                    if (i == members.Length)
                        i = 0;
                    if (_GetStrategy(memeber, strategys) == false)
                        break;
                }
                DoneEvent();
            }

            private bool _GetStrategy(Team.Member memeber, Strategy[] strategys)
            {
                for (int i = 0; i < strategys.Length; ++i )
                {
                    if (memeber.Teammate.Specializes == strategys[i])
                    {
                        memeber.Owner.AddStrategy(strategys[i]);
                        strategys[i] = Strategy.None;
                        return true;
                    }
                }
                for (int i = 0; i < strategys.Length; ++i)
                {
                    if (strategys[i] != Strategy.None)
                    {
                        memeber.Owner.AddStrategy(strategys[i]);
                        strategys[i] = Strategy.None;
                        return true;
                    }                    
                }
                return false;
            }

            private Team.Member[] _GetSurvivor(Team[] teams)
            {
                List<Team.Member> teammates = new List<Team.Member>();
                foreach (var team in teams)
                {
                    teammates.AddRange(from member in  team.Members where member.Teammate.Hp > 0 select member);
                }
                return teammates.OrderBy(t => t.Teammate.Int + Regulus.Utility.Random.Next(-2 , 2)).ToArray();
            }

            private Strategy[] _Generate()
            {
                int count = _GetTeamCount(_Teams) * 2 - 1;

                Strategy[] sourceStrategys = new Strategy[] { Strategy.Ax, Strategy.Shield, Strategy.Staff, Strategy.Sword };
                Strategy[] strategys = new Strategy[count];
                for (int i = 0; i < count; ++i)
                {
                    strategys[i] = sourceStrategys[Regulus.Utility.Random.Next(0, sourceStrategys.Length)];
                }
                return strategys;
            }

            private int _GetTeamCount(Team[] _Teams)
            {
                return (from team in _Teams select team.Members.Length).Sum();
            }

            void Regulus.Game.IStage.Leave()
            {
                
            }

            void Regulus.Game.IStage.Update()
            {
                
            }
        }
    }

}
