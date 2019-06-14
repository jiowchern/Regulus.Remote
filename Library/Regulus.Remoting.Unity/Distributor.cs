
using System.Collections.Generic;



namespace Regulus.Remote.Unity
{
    public class Distributor 
    {
        private readonly IAgent _Agent;

        

        private readonly Dictionary<int, Assigner> _Notifiers;

        public Distributor(IAgent agent)
        {
            _Agent = agent;
            _Notifiers = new Dictionary<int, Assigner>();
        }

           
        public void Attach<T>(Adsorber<T> adsorber)
        {
            var notifier = _Agent.QueryNotifier<T>();
            var notifier1 = _QueryNotifier(notifier);
            notifier1.Register(adsorber);
        }

        public void Detach<T>(Adsorber<T> adsorber)
        {
            var notifier = _Agent.QueryNotifier<T>();
            var notifier1 = _QueryNotifier(notifier);
            notifier1.Unregister(adsorber);
        }

        private Assigner _QueryNotifier<T>(INotifier<T> notifier)
        {
            var hash = notifier.GetHashCode();
            Assigner outAssigner;
            if (_Notifiers.TryGetValue(hash, out outAssigner))
            {
                return outAssigner;
            }

            outAssigner = new Assigner<T>(notifier);
            _Notifiers.Add(hash, outAssigner);
            return outAssigner;
        }

        public INotifier<T> QueryNotifier<T>()
        {
            return _Agent.QueryNotifier<T>();
        }
    }
}
