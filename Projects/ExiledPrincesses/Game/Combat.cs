using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
   
    partial class Combat
    {
        public class Team
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
            private Contingent.FormationType _Formation;
            private ITeammate[] _Teammates;
            
            public Team(Contingent.FormationType _Formation, ITeammate[] _Teammates)
            {                
                this._Formation = _Formation;
                this._Teammates = _Teammates;
            }

            public Member[] Members { get { return (from  t in _Teammates select new Member(this , t)).ToArray(); } }

            internal void AddStrategy(Strategy strategy)
            {
                throw new NotImplementedException();
            }
        }


        Regulus.Game.StageMachine _StageMachine;
        internal void Update()
        {
            _StageMachine.Update();
        }
        Team[] _Teams;
        internal void Initial(Team team1, Team team2)
        {
            _Teams = new Team[] { team1 , team2 };
            _StageMachine = new Regulus.Game.StageMachine();
            _ToStrategy(_Teams);
        }

        private void _ToStrategy(Team[] teams)
        {
            var stage = new StrategyStage(teams);
            stage.DoneEvent += _ToAction;
            _StageMachine.Push(stage);
        }

        private void _ToAction()
        {
            throw new NotImplementedException();
        }

        internal void Finial()
        {
            _StageMachine.Termination();
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
                var members = _GetMembers(_Teams);

                for (int i = 0; i < members.Length; ++i )
                {
                    var memeber = members[i];

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

            private Team.Member[] _GetMembers(Team[] teams)
            {
                List<Team.Member> teammates = new List<Team.Member>();
                foreach (var team in teams)
                {
                    teammates.AddRange(team.Members);
                }
                return teammates.OrderBy(t => t.Teammate.Dex ).ToArray();
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
                throw new NotImplementedException();
            }

            void Regulus.Game.IStage.Update()
            {
                throw new NotImplementedException();
            }
        }
    }

}
