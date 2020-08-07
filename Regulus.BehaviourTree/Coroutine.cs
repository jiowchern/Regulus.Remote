using Regulus.Utility;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Regulus.BehaviourTree.Yield
{
    public enum INSTRUCTION
    {
        DONE_BY_SUCCESS,
        DONE_BY_FAILURE,
        NEXT,
        WAIT
    }

    public interface IInstructable
    {
        INSTRUCTION Result();
    }


    public class Success : IInstructable
    {
        INSTRUCTION IInstructable.Result()
        {
            return INSTRUCTION.DONE_BY_SUCCESS;
        }
    }




    public class Failure : IInstructable
    {
        INSTRUCTION IInstructable.Result()
        {
            return INSTRUCTION.DONE_BY_FAILURE;
        }
    }

    public class WaitSeconds : IInstructable
    {
        private readonly float _Seconds;
        private readonly Regulus.Utility.TimeCounter _TimeCounter;
        public WaitSeconds(float seconds)
        {
            _TimeCounter = new TimeCounter();
            _Seconds = seconds;
        }
        INSTRUCTION IInstructable.Result()
        {
            if (_TimeCounter.Second > _Seconds)
                return INSTRUCTION.NEXT;
            return INSTRUCTION.WAIT;
        }
    }
    public class Wait : IInstructable
    {

        INSTRUCTION IInstructable.Result()
        {
            return INSTRUCTION.NEXT;
        }
    }

    public class WaitUntil : IInstructable
    {
        private readonly Func<bool> _Condition;

        public WaitUntil(Func<bool> condition)
        {
            _Condition = condition;
        }
        INSTRUCTION IInstructable.Result()
        {
            return _Condition() ? INSTRUCTION.NEXT : INSTRUCTION.WAIT;
        }
    }

    public class Coroutine : ITicker
    {

        private readonly IEnumerable<IInstructable> _Provider;
        private IInstructable _Current;
        private IEnumerator<IInstructable> _Action;
        private readonly Guid _Id;
        private readonly string _Tag;


        public Coroutine(Expression<Func<IEnumerable<IInstructable>>> expression)
        {
            _Id = Guid.NewGuid();
            MethodCallExpression methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression == null)
                throw new NotSupportedException(string.Format("The expression is a {0} , must a {1}", expression.Body.NodeType, ExpressionType.Call));

            _Tag = methodCallExpression.Method.Name;



            _Provider = expression.Compile()();

            _Reset();
        }

        Guid ITicker.Id { get { return _Id; } }
        string ITicker.Tag { get { return _Tag; } }
        ITicker[] ITicker.GetChilds()
        {
            return new ITicker[0];
        }

        void ITicker.GetPath(ref List<Guid> nodes)
        {
            nodes.Add(_Id);
        }


        void _Reset()
        {
            _Action = _Provider.GetEnumerator();
            _Current = null;
        }
        void ITicker.Reset()
        {
            _Reset();

        }

        TICKRESULT ITicker.Tick(float delta)
        {
            if (_Current != null)
            {
                INSTRUCTION result = _Current.Result();
                if (result == INSTRUCTION.DONE_BY_FAILURE || result == INSTRUCTION.DONE_BY_SUCCESS)
                {
                    _Reset();
                    return result == INSTRUCTION.DONE_BY_SUCCESS ? TICKRESULT.SUCCESS : TICKRESULT.FAILURE;
                }
                if (result == INSTRUCTION.NEXT)
                {
                    _Current = _Next();
                    return TICKRESULT.RUNNING;
                }
                if (result == INSTRUCTION.WAIT)
                {
                    return TICKRESULT.RUNNING;
                }
            }
            _Current = _Next();
            if (_Current != null)
                return TICKRESULT.RUNNING;

            _Reset();
            return TICKRESULT.SUCCESS;
        }

        private IInstructable _Next()
        {
            if (_Action.MoveNext())
                return _Action.Current;
            return null;
        }
    }
}

namespace Regulus.BehaviourTree.ActionHelper
{
    public class Coroutine : ITicker
    {
        private readonly IEnumerable<TICKRESULT> _Provider;
        private IEnumerator<TICKRESULT> _Action;


        private readonly Guid _Id;
        private readonly string _Tag;

        public Coroutine(Expression<Func<IEnumerable<TICKRESULT>>> expression)
        {
            _Id = Guid.NewGuid();
            MethodCallExpression methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression == null)
                throw new NotSupportedException(string.Format("The expression is a {0} , must a {1}", expression.Body.NodeType, ExpressionType.Call));

            _Tag = methodCallExpression.Method.Name;

            _Provider = expression.Compile()();
            _Action = _Provider.GetEnumerator();
        }


        Guid ITicker.Id { get { return _Id; } }
        string ITicker.Tag { get { return _Tag; } }

        ITicker[] ITicker.GetChilds()
        {
            return new ITicker[0];
        }

        void ITicker.GetPath(ref List<Guid> nodes)
        {
            nodes.Add(_Id);
        }

        void ITicker.Reset()
        {
            _Action = _Provider.GetEnumerator();
        }

        TICKRESULT ITicker.Tick(float delta)
        {

            if (_Action.MoveNext())
            {
                TICKRESULT result = _Action.Current;
                if (result != TICKRESULT.RUNNING)
                {
                    _Action = _Provider.GetEnumerator();
                }
                return result;
            }
            return TICKRESULT.RUNNING;
        }


    }
}