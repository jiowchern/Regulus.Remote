namespace Regulus.Remote.Client
{
    public class ConnectSet<TConnecter>
    {
        public ConnectSet(TConnecter listener, Ghost.IAgent agent)
        {
            Connecter = listener;
            Agent = agent;
        }

        public readonly TConnecter Connecter;
        public readonly Ghost.IAgent Agent;

    }
}