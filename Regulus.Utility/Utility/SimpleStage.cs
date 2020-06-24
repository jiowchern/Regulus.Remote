using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
    public class SimpleStage<T> : IStatus<T>
    {
        readonly Action _EnterEvent;


        readonly Action<T> _UpdateEvent;
        readonly Action _LeaveEvent;

        public SimpleStage(Action enter)
        {
            _EnterEvent = enter;
            _UpdateEvent = (arg) => { };
            _LeaveEvent = () => { };
        }

        public SimpleStage(Action enter, Action leave)
        {
            _EnterEvent = enter;
            _UpdateEvent = (arg) => { };
            _LeaveEvent = leave;
        }
        public SimpleStage(Action enter ,  Action leave , Action<T> update)
        {
            _EnterEvent = enter;
            _UpdateEvent = update;
            _LeaveEvent = leave;
        }
        void IStatus<T>.Enter()
        {
            _EnterEvent();
        }

        void IStatus<T>.Leave()
        {
            _LeaveEvent();
        }

        void IStatus<T>.Update(T arg)
        {
            _UpdateEvent(arg);
        }
    }



    public class SimpleStage : IStatus
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
        public SimpleStage(Action enter, Action leave, Action update)
        {
            _EnterEvent = enter;
            _UpdateEvent = update;
            _LeaveEvent = leave;
        }
        void IStatus.Enter()
        {
            _EnterEvent();
        }

        void IStatus.Leave()
        {
            _LeaveEvent();
        }

        void IStatus.Update()
        {
            _UpdateEvent();
        }
    }
}
