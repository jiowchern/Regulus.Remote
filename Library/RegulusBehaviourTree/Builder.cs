using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Regulus.BehaviourTree
{
    public class Builder
    {
        private ITicker _Root;

        private Stack<IParent> _Stack;
        public Builder()
        {
            _Stack = new Stack<IParent>();
        }

        public Builder Action(IEnumerable<TICKRESULT> provider)
        {
            this.Action(new ActionHelper.Coroutine(provider));
            return this;
        }
        public Builder Action<T>(Expression<Func<T>> instnace
            , Expression<Func<T, Func<float, TICKRESULT>>> tick
            , Expression<Func<T, Action>> start
            , Expression<Func<T, Action>> end)
        {
            
            if (_Stack.Count <= 0)
            {
                throw new Exception("Can't create an unnested ActionNode, it must be a leaf node.");
            }
            var a = new ActionNode<T>(instnace, tick, start, end);
            _Stack.Peek().Add(a);
            return this;

        }

        public Builder Sub(ITicker node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (_Stack.Count <= 0)
            {
                throw new ApplicationException("Can't splice an unnested sub-tree, there must be a parent-tree.");
            }

            _Stack.Peek().Add(node);
            return this;
        }

        public Builder Not()
        {
            var node = new InvertNode();

            if (_Stack.Count > 0)
            {
                _Stack.Peek().Add(node);
            }

            _Stack.Push(node);
            return this;
        }

        public Builder Success()
        {
            var node = new SuccessNode();

            if (_Stack.Count > 0)
            {
                _Stack.Peek().Add(node);
            }

            _Stack.Push(node);
            return this;
        }

        public Builder Sequence()
        {
            var sequenceNode = new SequenceNode();

            if (_Stack.Count > 0)
            {
                _Stack.Peek().Add(sequenceNode);
            }

            _Stack.Push(sequenceNode);
            return this;
        }

        public Builder Selector()
        {
            var selectorNode = new SelectorNode();

            if (_Stack.Count > 0)
            {
                _Stack.Peek().Add(selectorNode);
            }

            _Stack.Push(selectorNode);
            return this;
        }

        public Builder Parallel( int num_required_to_fail, int num_required_to_succeed)
        {
            var parallelNode = new ParallelNode( num_required_to_fail, num_required_to_succeed);

            if (_Stack.Count > 0)
            {
                _Stack.Peek().Add(parallelNode);
            }

            _Stack.Push(parallelNode);
            return this;
        }

        public ITicker Build()
        {
            if (_Root == null)
            {
                throw new ApplicationException("Can't create a behaviour tree with zero nodes");
            }
            return _Root;
        }

        public Builder End()
        {
            _Root = _Stack.Pop();
            return this;
        }

        class TickAction
        {
            private readonly Func<float, TICKRESULT> _Func;

            public TickAction(Func<float, TICKRESULT> func)
            {
                _Func = func;
            }

            public TICKRESULT Tick(float arg)
            {
                var result = _Func(arg);
                return result;
            }

            public void Start()
            {

            }

            public void End()
            {

            }
        }

        public Builder Action(IAction action)
        {
            this.Action(
                () => action
                , c => c.Tick
                , c => c.Start
                , c => c.End
            );
            return this;
        }
        public Builder Action(Expression<Func<IAction>> action)
        {
            this.Action(
                () => new ProxyAction(action)
                , c => c.Tick
                , c => c.Start
                , c => c.End
                );
            return this;
        }
        public Builder Action(Func<float, TICKRESULT> func)
        {

            this.Action(
                ()=> new TickAction(func)
                , c => c.Tick
                , c => c.Start
                , c => c.End
                );
            return this;
        }
    }
}
