using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class SimpleStage : IStage
    {
        readonly Action _EnterEvent;


        readonly Action _UpdateEvent;
        readonly Action _LeaveEvent;

        public SimpleStage(Action enter)
        {
            _EnterEvent = enter;
            _UpdateEvent = () => { };
            _LeaveEvent = () => { };
        }

        public SimpleStage(Action enter, Action leave)
        {
            _EnterEvent = enter;
            _UpdateEvent = () => { };
            _LeaveEvent = leave;
        }
        public SimpleStage(Action enter ,  Action leave , Action update)
        {
            _EnterEvent = enter;
            _UpdateEvent = update;
            _LeaveEvent = leave;
        }
        void IStage.Enter()
        {
            _EnterEvent();
        }

        void IStage.Leave()
        {
            _LeaveEvent();
        }

        void IStage.Update()
        {
            _UpdateEvent();
        }
    }
}
