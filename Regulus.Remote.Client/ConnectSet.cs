namespace Regulus.Remote.Client
{
    public class ConnectSet<TConnecter>
    {
        public ConnectSet(TConnecter listener, Ghost.IAgent agent)
        {
            Connector = listener;
            Agent = agent;
        }

        public readonly TConnecter Connector;
        public readonly Ghost.IAgent Agent;

    }
}