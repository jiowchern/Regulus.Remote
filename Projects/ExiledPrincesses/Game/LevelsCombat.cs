using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    partial class Levels
    {
        class CombatStage : Regulus.Game.IStage
        {
            public enum Result
            {
                Victory, Failure
            }
            public delegate void OnResult(Result result);
            public event OnResult ResultEvent;

            
            Combat _Combat;
            BattlefieldPrototype _Prototype;
            Platoon _Platoon;
            public CombatStage(BattlefieldPrototype prototype, Platoon platoon)
            {
                _Prototype = prototype;
                
                _Platoon = platoon;

                _Combat = new Combat();
            }
            void Regulus.Game.IStage.Enter()
            {
                var team1 = new Team(1,_Platoon);
                
                var enemys = (from enemy in _Prototype.Enemys select new Teammate(new ActorInfomation() { Exp = 0, Prototype = enemy })).ToArray();

                var platoonEnemy = new Platoon(new Squad(Contingent.FormationType.Auxiliary, enemys , new AIController()));
                var team2 = new Team(2,platoonEnemy);
                _Combat.Initial(team1, team2);
                _Combat.WinnerEvent += (winner) =>
                {
                    if (winner == team1)
                        ResultEvent(Result.Victory);
                    else
                        ResultEvent(Result.Failure);
                };

                _Combat.DrawEvent += () =>
                {
                    ResultEvent(Result.Failure);
                };

            }

            void Regulus.Game.IStage.Leave()
            {
                _Combat.Finial();
            }

            void Regulus.Game.IStage.Update()
            {
                _Combat.Update();
            }
        }
    }
}
