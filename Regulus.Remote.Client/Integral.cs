namespace Regulus.Remote.Client
{
    public class Integral<TConnecter>
    {
        public Integral(TConnecter listener, Ghost.IAgent agent)
        {
            Connecter = listener;
            Agent = agent;
        }

        public readonly TConnecter Connecter;
        public readonly Ghost.IAgent Agent;

    }
}