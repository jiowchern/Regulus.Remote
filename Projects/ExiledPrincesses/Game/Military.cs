using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Regulus.Project.ExiledPrincesses.Game
{
    /*
     *  一般來說，以歐洲的編制方式來說，就是：
        集團軍（Army group）軍團(Army) 軍(Korps) 師(Divisions)
        旅(Brigade)/團(Regiment) 營(Battallion) 連(Company)
        排(platoon) 班(squad)
     */

    public interface ICommandable
    {
        void AuthorizeIdle(IAdventureIdle adventure_idel);
        void InterdictIdle(IAdventureIdle adventure_idel);
        void AuthorizeChoice(IAdventureChoice adventure_choice);
        void InterdictChoice(IAdventureChoice adventure_choice);
        
    }
    public class Squad : ICommandable
    {
        ITeammate[] _ITeammates;
        public IActor[] Actors
        {
            get{ return _ITeammates; }
        }
        Controller _Controller;
        public Squad(ITeammate[] teammates , Controller controller)
        {
            _ITeammates = teammates;
            _Controller = controller;
        }
        void ICommandable.AuthorizeIdle(IAdventureIdle adventureIdle)
        {
            _Controller.SetIdleController(adventureIdle);
        }

        void ICommandable.InterdictIdle(IAdventureIdle adventureIdle)
        {
            _Controller.SetIdleController(null);
        }

        public void Go(IAdventureGo adventure_go)
        {
            _Controller.SetGoController(adventure_go);
        }
        public void Stop()
        {
            _Controller.SetGoController(null);
        }


        void ICommandable.AuthorizeChoice(IAdventureChoice adventure_choice)
        {
            _Controller.SetChoiceController(adventure_choice);
        }

        void ICommandable.InterdictChoice(IAdventureChoice adventure_choice)
        {
            _Controller.SetChoiceController(null);
        }

        public void SetComrades(IActor[] actors)
        {
            _Controller.SetComrades(actors);
        }
    }

    public class Platoon : ICommandable , Regulus.Utility.IUpdatable
    {
        
        private Squad _Squad;

        public delegate void OnEmpty();
        public event OnEmpty EmptyEvent;
        public Platoon(Squad squad)
        {            
            this._Squad = squad;
        }


        void ICommandable.AuthorizeIdle(IAdventureIdle adventureIdle)
        {
            (_Squad as ICommandable).AuthorizeIdle(adventureIdle);
        }

        void ICommandable.InterdictIdle(IAdventureIdle adventureIdle)
        {
            (_Squad as ICommandable).InterdictIdle(adventureIdle);
        }

        public void Go(IAdventureGo adventure_go)
        {
            _Squad.Go(adventure_go);
        }
        public void Stop()
        {
            _Squad.Stop();
        }


        void ICommandable.AuthorizeChoice(IAdventureChoice adventure_choice)
        {
            (_Squad as ICommandable).AuthorizeChoice(adventure_choice);
        }

        void ICommandable.InterdictChoice(IAdventureChoice adventure_choice)
        {
            (_Squad as ICommandable).InterdictChoice(adventure_choice);
        }

        internal void Leave(Squad squad)
        {
            if (squad == _Squad)
            {
                EmptyEvent();
                _Squad = null;
            }
        }

        bool Utility.IUpdatable.Update()
        {
            return true;
        }

        void Framework.ILaunched.Launch()
        {
            IActor[] actors = _GetTeammates();

            _Squad.SetComrades(actors);
        }

        private IActor[] _GetTeammates()
        {
            return _Squad.Actors;
        }

        void Framework.ILaunched.Shutdown()
        {
            _Squad.SetComrades(new IActor[0]);
        }
    }
}
