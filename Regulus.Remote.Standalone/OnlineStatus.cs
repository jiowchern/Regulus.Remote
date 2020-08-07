using Regulus.Utility;

namespace Regulus.Remote.Standalone
{
    internal class OnlineStatus : IStatus
    {
        private readonly IProvider _Provider;
        public readonly OnlineGhost Ghost;
        public OnlineStatus(IProvider provider, OnlineGhost ghost)
        {
            this._Provider = provider;

            Ghost = ghost;
        }

        void IStatus.Enter()
        {
            _Provider.Add(Ghost);
            _Provider.Ready(Ghost.Id);
        }

        void IStatus.Leave()
        {
            _Provider.Remove(Ghost.Id);
        }

        void IStatus.Update()
        {

        }
    }
}