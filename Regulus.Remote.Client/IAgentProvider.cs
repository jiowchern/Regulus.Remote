namespace Regulus.Remote.Client
{
    public interface IAgentProvider
    {
        IAgent Spawn();
    }
}