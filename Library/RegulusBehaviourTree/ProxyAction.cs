using System;
using System.Linq.Expressions;

namespace Regulus.BehaviourTree
{
    public class ProxyAction
    {
        private readonly Expression<Func<IAction>> _ActionProvider;

        private IAction _Action;

        public ProxyAction(Expression<Func<IAction>> action_provider)
        {
            _ActionProvider = action_provider;

            _Action = _ActionProvider.Compile()();
        }

        public TICKRESULT Tick(float arg)
        {
            return _Action.Tick(arg);
        }

        public void Start()
        {
            _Action.Start();
        }

        public void End()
        {
            _Action.End();
        }
    }
}