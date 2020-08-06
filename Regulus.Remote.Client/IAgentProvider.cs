namespace Regulus.Remote.Client
{
    public interface IAgentProvider
    {
        Ghost.IAgent Spawn();
    }
}