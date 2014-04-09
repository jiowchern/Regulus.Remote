using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    internal interface IBehaviorHandler : Regulus.Utility.IUpdatable, IBehaviorCommandInvoker
    {
    }

    abstract public class BehaviorStage : IBehaviorStage
    {
        Regulus.Utility.TUpdater<IBehaviorHandler> _Updater;
        

        abstract internal ITriggerableAbility _TriggerableAbility();
        abstract internal IBehaviorHandler[] _Handlers();

        abstract protected void _Begin();
        abstract protected void _End();

        protected BehaviorStage()
        {
            _Updater = new Utility.TUpdater<IBehaviorHandler>();
        }

        private void _BuildHandler(IBehaviorHandler[] behavior_handlers)
        {
            _Updater = new Utility.TUpdater<IBehaviorHandler>();

            foreach (var handles in behavior_handlers)
            {
                _Updater.Add(handles);
            }
        }

        ITriggerableAbility IBehaviorStage.Ability
        {
            get { return _TriggerableAbility(); }
        }

        void Game.IStage.Enter()
        {
            _Begin();
            _BuildHandler(_Handlers());
        }



        void Game.IStage.Leave()
        {
            _Updater.Shutdown();
            _End();
        }

        void Game.IStage.Update()
        {
            _Updater.Update();
        }

        void IBehaviorCommandInvoker.Invoke(IBehaviorCommand command)
        {
            foreach (var obj in _Updater.Objects)
            {
                obj.Invoke(command);
            }
        }
    }
}
