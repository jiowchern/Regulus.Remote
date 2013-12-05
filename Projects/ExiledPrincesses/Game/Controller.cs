using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public interface IController
    {
        void BattleBegins();
        void BattleEnd();
        void SetIdleController(IAdventureIdle adventure_idle);
        void SetGoController(IAdventureGo adventure_go);
        void SetChoiceController(IAdventureChoice adventure_choice);
        void SetEnemys(IActor[] actors);
        void SetComrades(IActor[] actors);
        void SetTeamss(ITeam[] teams);
        void SetCombatController(ICombatController[] controllers);    
    }

    public class AIController : IController
    {
        void IController.SetIdleController(IAdventureIdle adventure_idle)
        {
            
        }

        void IController.SetGoController(IAdventureGo adventure_go)
        {
            
        }

        void IController.SetChoiceController(IAdventureChoice adventure_choice)
        {
            
        }

        void IController.SetComrades(IActor[] actors)
        {
            
        }

        void IController.SetTeamss(ITeam[] teams)
        {
            
        }

        
        void IController.SetEnemys(IActor[] actors)
        {
            
        }


        void IController.SetCombatController(ICombatController[] controllers)
        {
            
        }

        void IController.BattleBegins()
        {
            
        }

        void IController.BattleEnd()
        {
            
        }
    }
}
