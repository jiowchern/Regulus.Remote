namespace Regulus.Profiles.StandaloneAllFeature.Console
{
    class User
    {
        public readonly Regulus.Remote.Ghost.IAgent Agent;
        public readonly int Id;
        public long Ticks;
        public User(Regulus.Remote.Ghost.IAgent agent, int id)
        {
            Agent = agent;
            Id = id;
        }
    }
}
