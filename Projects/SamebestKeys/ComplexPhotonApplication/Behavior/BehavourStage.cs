using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    public class BehaviorStage : Regulus.Utility.IStage
    {
        Regulus.Utility.TUpdater<IBehaviorHandler> _Updater;

        internal delegate IBehaviorHandler[] OnBegin();
        internal event OnBegin BeginEvent;
        internal delegate void OnEnd();
        internal event OnEnd EndEvent;
        private OnBegin begin;

        internal BehaviorStage()
        {
            _Updater = new Utility.TUpdater<IBehaviorHandler>();
        }

        internal BehaviorStage(OnBegin begin)
            : this()
        {
            BeginEvent += begin;
        }

        private void _BuildHandler(IBehaviorHandler[] behavior_handlers)
        {
            _Updater = new Utility.TUpdater<IBehaviorHandler>();

            foreach (var handles in behavior_handlers)
            {
                _Updater.Add(handles);
            }
        }

        void Utility.IStage.Enter()
        {
            if (BeginEvent != null)
                _BuildHandler(BeginEvent());
        }



        void Utility.IStage.Leave()
        {
            if (EndEvent != null)
                EndEvent();
            _Updater.Shutdown();
        }

        void Utility.IStage.Update()
        {
            _Updater.Update();
        }


    }
}
